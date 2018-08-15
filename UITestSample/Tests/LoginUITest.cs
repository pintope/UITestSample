namespace UITestSample.Tests
{
    using System;
    using System.Threading;
    using System.Configuration;
    using System.Windows.Automation;
    using Pintope.ViewAutomation;


    /// <summary>
    /// Login test.
    /// </summary>
    public class LoginUITest
    {
        private readonly string user;
        private readonly string password;
        private readonly StructureChangedEventHandler structureChangedEventHandler;
        private static AutoResetEvent eventControl;


        public LoginUITest(AutoResetEvent eventControl = null)
        {
            user = ConfigurationManager.AppSettings["user"];
            password = ConfigurationManager.AppSettings["password"];
            structureChangedEventHandler = new StructureChangedEventHandler(OnStructureChanged);
            LoginUITest.eventControl = eventControl;
        }


        /// <summary>
        /// Runs the login test.
        /// </summary>
        public bool Login()
        {
            try
            {
                // Looks for and invoke the login button.
                AutomationElement node = GetAnchor2(true);
                ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.AutomationIdProperty, "LoginButton") }, true, Pattern.Invoke);

                // Enters the email address.
                node = GetAnchor2();
                ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.AutomationIdProperty, "EmailTextBox") }, true, Pattern.Value, user);

                // Enters the password.
                node = GetAnchor2();
                ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.AutomationIdProperty, "PasswordBox") }, true, Pattern.Value, password);

                // Before sending login credentials to the server, we must first subscribe to the structucture changed event at the level of anchor #1.
                // This way we detect a successful login.
                node = GetAnchor1();
                Automation.AddStructureChangedEventHandler(node, TreeScope.Children, structureChangedEventHandler);

                // Looks for and invoke the sign in button.
                node = GetAnchor2();
                ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.AutomationIdProperty, "SignInButton") }, true, Pattern.Invoke);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// In the login window, gets the parent's parent node of main form, from which we can search controls.
        /// </summary>
        /// <param name="setFocus">Whether we must bring the login window to the foreground or not.</param>
        private AutomationElement GetAnchor1(bool setFocus = false)
        {
            // Retrieves login window.
            AutomationElement node = AutomationElement.RootElement;
            ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.NameProperty, "Wunderlist") });

            // Brings login window to the foreground.
            if (setFocus) ViewTree.Focus(node);

            return node;
        }

        /// <summary>
        /// In the login window, gets the parent node of main form, from which we can search controls.
        /// </summary>
        /// <param name="setFocus">Whether we must bring the login window to the foreground or not.</param>
        private AutomationElement GetAnchor2(bool setFocus = false)
        {
            AutomationElement node = GetAnchor1(setFocus);
            ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.ClassNameProperty, "Windows.UI.Core.CoreWindow") });
            ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.ClassNameProperty, "ScrollViewer") });
            return node;
        }

        /// <summary>
        /// Event handler for the structure changed event.
        /// </summary>
        private static void OnStructureChanged(object src, AutomationEventArgs e)
        {
            if ((src as AutomationElement).Current.Name == "Wunderlist")
            {
                eventControl?.Set();
            }
        }
    }
}