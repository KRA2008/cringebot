using System;
using System.Net;
using Foundation;
using Social;
using UIKit;

namespace ShareExtension
{
    public partial class ShareViewController : SLComposeServiceViewController
    {
        public ShareViewController(IntPtr handle) : base(handle) {}

        public override bool IsContentValid()
        {
            // Do validation of contentText and/or NSExtensionContext attachments here
            
            return true;
        }

        public override void DidSelectPost()
        {
            const string URL = "cringebot://cringebot?";
            var url = new NSUrl(URL + WebUtility.UrlEncode(ContentText));
            UIApplication.SharedApplication.OpenUrl(url);

            ExtensionContext.CompleteRequest(new NSExtensionItem[0], null);
        }

        public override SLComposeSheetConfigurationItem[] GetConfigurationItems()
        {
            // To add configuration options via table cells at the bottom of the sheet, return an array of SLComposeSheetConfigurationItem here.
            return new SLComposeSheetConfigurationItem[0];
        }
    }
}
