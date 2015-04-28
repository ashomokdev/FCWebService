using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace FCWebService
{
    public class Request
    {
        internal Request(byte[] docDifinition, List<byte[]> images)
        {
            _startRequestLength = images.Count;
            DocDifinition = docDifinition;
            Images = new ConcurrentQueue<byte[]>(images);
            GUID = Guid.NewGuid().ToString();
        }

        internal List<PersonData> Result
        {
            get { return _result; }
        }

        internal string GUID { get; set; }

        internal byte[] DocDifinition { get; set; }

        internal ConcurrentQueue<byte[]> Images { get; set; }

        public string Error { get; set; }

        private List<PersonData> _result = new List<PersonData>();
        private readonly int _startRequestLength;

        public AutoResetEvent requestFinished = new AutoResetEvent(false);
        internal bool TryGetRequestPiece(int taskSize, out List<byte[]> requestPiece)
        {
            requestPiece = new List<byte[]>();
            //if, for example, taskSize == 5, but _images.Count == 3 it returns only 3 images.
            //this images was deleted from parent list.
            for (int i = 0; i < taskSize; i++)
            {
                byte[] image;
                Images.TryDequeue(out image);
                if (image != null)
                {
                    requestPiece.Add(image);
                }
            }

            if (requestPiece.Count == 0)
            {
                Schedule.Instance.TryToAddRequestsToWork();
                return false;
            }
            else return true;
        }

        internal void AddCheckResult(PersonData item)
        {
            lock (_result)
            {
                Result.Add(item);
                if (Result.Count == _startRequestLength)
                {
                    requestFinished.Set();
                }
            }
        }
    }
}
