
#WebService on FlaxiCapture Engine ABBYY SDK
WebService on IIS which use OCR for obtaining info of pasport scans.
Solution contains of 3 projects:

1. **FCWebService:** <br>
   Simple high-load Web Service, which implemented with using RoundRobin Algorithm. <br>
   It also containing working with:<br>
    * xdocument (xml reading/parsing) 
    * ConcurrentQueue 
    * AutoResetEvent (waiting for request finished)
  
2.  **WebSite:**<br>
    ASP.NET WebForms WebSite with few JQuery methods.
 
3. **FCUnitTest:**<br>
    Testing.

note:
FlexiCaptureProcessorsPool.cs, 
Config.cs
are missed here because of Privacy Policy of the Company
