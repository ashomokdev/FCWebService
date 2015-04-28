<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html>
<!-- Latest compiled and minified CSS -->
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css" />
<html xmlns="http://www.w3.org/1999/xhtml">

<head>
    <script src="Scripts\jquery-1.9.0.js"></script>

    <title>FC Service Test Site</title>
    <style>
        body {
            padding-top: 20px;
        }

        .margin-base-vertical {
            margin: 40px 0;
        }
    </style>
</head>

<body>
    <form role="form" id="formDefault" runat="server">
        <div class="container">
            <div class="col-md-6 col-md-offset-3 panel panel-default">
                <h1 class="margin-base-vertical text-center">FC Service TestSite</h1>

                <div class="multiUserScenario" id="multiUserScenario">
                    <asp:Panel ID="panelInputDocDif" runat="server">
                        <label for="inputDocumentDefinition"><small>Pls add Document Difinition (Optional):</small></label>
                        <asp:FileUpload accept=".xml" ID="inputDocumentDefinition" runat="server" />
                        <p class="help-block" id="documentDefinitionWarning" style="color: orange">Warning: Default Document Definition will be used.</p>
                        <p class="help-block" id="documentDefinitionExtensionError" style="color: red; display: none">Error: This extension not supported. Try to load .xml.</p>
                    </asp:Panel>

                    <label for="inputImages"><small>Pls add images:</small></label>
                    <asp:FileUpload runat="server" accept=".jpg" ID="inputImages" multiple="true" />
                    <p class="help-block" id="imageErrorBadExtension" style="color: red; display: none">Error: This extension not supported. Try to load .jpg.</p>

                    <asp:Label runat="server" Style="color: red" Visible="false" ID="labelImagesError"></asp:Label>
                    <asp:Label runat="server" Style="color: orange" Visible="false" ID="labelImagesWarning"></asp:Label>
                    <p class="text-center">
                        <asp:Button ID="submitButton" runat="server" CssClass="btn btn-default" OnClick="ButtonGetInfo_Click" Text="Get passport info" Enabled="False" />
                    </p>

                    <asp:Panel ID="panelResultTable" runat="server" Visible="False">
                        <asp:Table ID="tableResult" runat="server" BorderWidth="1" GridLines="Both">
                            <asp:TableHeaderRow runat="server">
                                <asp:TableHeaderCell>Passport Number</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Expiry Date</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                        </asp:Table>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </form>
    <script src="/Scripts/FCServiceSite.js"></script>
</body>
</html>
