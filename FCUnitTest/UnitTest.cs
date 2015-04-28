using System;
using System.IO;
using FCUnitTest.ServiceReference1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FCUnitTest
{
    [TestClass()]
    public class UnitTest1
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        //Recomendation for creating Load Test: 3 users, 15 iterations.
        [TestMethod()]
        [DeploymentItem("images_for_testing", "images_for_testing")]
        [DeploymentItem("DocumentDefinition.xml")]
        public void MultithreadedProcessorUsingTest()
        {
            string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.log");
            TestContext.AddResultFile(pathToLog);
            WebService1SoapClient _service = new WebService1SoapClient();
            byte[] byteDocDefinition = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DocumentDefinition.xml"));

            var rand = new Random();
            var files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images_for_testing"), "*.jpg");

            ArrayOfBase64Binary arrayImages = new ArrayOfBase64Binary();
            for (int i = 0; i < rand.Next(5, 5); i++)
            {
                string randomImage = files[rand.Next(files.Length)];
                byte[] image = File.ReadAllBytes(randomImage);
                arrayImages.Add(image);
            }

            var response = _service.GetResultFromImages(byteDocDefinition, arrayImages);

            if (!File.Exists(pathToLog))
            {
                File.Create(pathToLog).Close();
            }

            using (StreamWriter streamWriter = File.AppendText(pathToLog))
            {
                string logMessage = "";
                foreach (var person in response)
                {
                    logMessage += "\nPassport: \n" + new string(person.Number.ToArray()) + "\n" + new string(person.Date.ToArray())
                        + "\n" + "Request GUID:" + person.processInfo.RequestGUID + "\n"
                        + "Processor number = " + person.processInfo.processNumber + "\n"
                        + "Free processors count = " + person.processInfo.freeProcCount + "\n";
                    if (person.processInfo.error != null)
                    {
                        logMessage += "Error : " + person.processInfo.error + "\n";
                    }
                }
                Log(logMessage, streamWriter);
            }
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine(logMessage);
            w.WriteLine("-------------------------------");
        }
    }
}