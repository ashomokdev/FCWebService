//ROUND ROBIN SCHEDULING ALGORITHM
using FCEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace FCWebService
{
    public class Schedule
    {
        public static Schedule Instance
        {
            get
            {
                return instance;
            }
        }

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static Schedule()
        {
        }

        private Schedule()
        {
            Thread processing = new Thread(StartWork);
            processing.Start();
        }

        private void StartWork()
        {
            while (true)
            {
                //TODO - candidate for refactoring. Need to sleep different time in different situation. If tryProcessTask() = true, go again without long waiting. 
                tryProcessTask();
                Thread.Sleep(300);
            }
        }

        private bool tryProcessTask()
        {
            bool result = false;
            if (requestsInWork.Count == 0 && requests.Count != 0)
            {
                TryToAddRequestsToWork();
            }

            //take actual request, cut, process task from it and put this request back to queue.
            Request actualRequest;
            bool actualRequestObtained = requestsInWork.TryDequeue(out actualRequest);
            if (actualRequestObtained)
            {
                result = true;
                List<byte[]> images;
                bool isRequestPieceObtained = actualRequest.TryGetRequestPiece(taskSize, out images);
                if (isRequestPieceObtained)
                {
                    requestsInWork.Enqueue(actualRequest);

                    int procNumb;
                    IEngine engine;
                    IFlexiCaptureProcessor processor = null;
                    processor = fcProcessorsPool.GetProcessor(out engine, out procNumb);

                    Tuple<IFlexiCaptureProcessor, IEngine, int, List<byte[]>, Request> startParams =
                        Tuple.Create(processor, engine, procNumb, images, actualRequest);
                    Thread recognition = new Thread(Recognition);
                    recognition.Start(startParams);
                }
            }
            return result;
        }

        private void Recognition(object startParam)
        {
            Tuple<IFlexiCaptureProcessor, IEngine, int, List<byte[]>, Request> startParams = startParam as Tuple<IFlexiCaptureProcessor, IEngine, int, List<byte[]>, Request>;
            try
            {
                string docDifPath = HelpUtils.ByteArrayToFile(Guid.NewGuid().ToString() + ".xml", startParams.Item5.DocDifinition);
                HelpUtils.DoConfigureProcessor(docDifPath, startParams.Item1, startParams.Item2);

                foreach (byte[] img in startParams.Item4)
                {
                    string fileName = Guid.NewGuid().ToString();
                    string imagePath = HelpUtils.ByteArrayToFile(fileName + ".jpg", img);
                    startParams.Item1.AddImageFile(imagePath);
                }
                int size = taskSize;
                while (size > 0)
                {
                    PersonData personData = new PersonData();
                    try
                    {
                        IDocument document = startParams.Item1.RecognizeNextDocument();
                        size--;
                        if (document == null)
                        {
                            IProcessingError processingError = startParams.Item1.GetLastProcessingError();
                            if (processingError != null)
                            {
                                throw new Exception(processingError.MessageText());
                            }
                        }
                        else if (document.DocumentDefinition == null)
                        {
                            throw new Exception("Can not get information.");
                        }

                        personData.Number = document.Sections[0].Children[0].Value.AsString.ToCharArray();
                        personData.Date = (document.Sections[0].Children[1].Value.AsString + " " + document.Sections[0].Children[2].Value.AsString).ToCharArray(); //Day + MonthYear
                        personData.processInfo.processNumber = startParams.Item3;
                        personData.processInfo.freeProcCount = fcProcessorsPool.FreeProcCount;
                        personData.processInfo.RequestGUID = startParams.Item5.GUID;
                        //TODO File.Delete(imagePath);
                    }
                    catch (Exception ex)
                    {
                        personData.processInfo.error += ex.Message + ex.StackTrace + ex.Source;
                    }
                    startParams.Item5.AddCheckResult(personData);
                }
            }
            catch (Exception ex)
            {
                startParams.Item5.Error += ex.Message + ex.StackTrace + ex.Source;
            }
            finally
            {
                if (startParams.Item5.Error != null)
                {
                    //TODO : if request has  global errors - stop work with this request
                }
                if (startParams.Item2 != null)
                {
                    fcProcessorsPool.ReleaseProcessor(startParams.Item1);
                }
            }
        }

        internal void TryToAddRequestsToWork()
        {
            while (requestsInWork.Count < processorCount && requests.Count > 0)
            {
                Request request;
                bool isPossibleToGet = requests.TryDequeue(out request);
                if (isPossibleToGet)
                {
                    requestsInWork.Enqueue(request);
                }
            }
        }

        public void AddRequest(Request request)
        {
            requests.Enqueue(request);
        }

        private static readonly int processorCount = Config.ProcessorCount;
        public static FlexiCaptureProcessorsPool fcProcessorsPool = new FlexiCaptureProcessorsPool(processorCount);
        private static readonly Schedule instance = new Schedule();
        private static readonly int taskSize = 1;
        private ConcurrentQueue<Request> requests = new ConcurrentQueue<Request>();
        private ConcurrentQueue<Request> requestsInWork = new ConcurrentQueue<Request>();
    }
}