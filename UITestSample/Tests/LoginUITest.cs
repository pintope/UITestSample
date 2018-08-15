namespace Tests
{
    using System;
    using System.Threading;
    using System.Configuration;
    using System.Windows.Automation;
    using SogetiViewAutomation;


    /// <summary>
    /// Proxy que representa la ventana de la aplicación SEO.
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
        /// Ejecuta la prueba funcional.
        /// </summary>
        public bool Login()
        {
            try
            {
                // Busca e invoca el botón "Iniciar sesión".
                AutomationElement node = GetAnchor2(true);
                ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.NameProperty, "Iniciar sesión") }, true, Pattern.Invoke);

                // Introduce el correo electrónico.
                node = GetAnchor2();
                ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.AutomationIdProperty, "EmailTextBox") }, true, Pattern.Value, user);

                // Introduce la contraseña.
                node = GetAnchor2();
                ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.AutomationIdProperty, "PasswordBox") }, true, Pattern.Value, password);

                // Antes de enviar las credenciales al servidor, la aplicación se suscribe al evento de apertura de ventana al nivel 
                // del nodo raíz para detectar el login exitoso.
                node = GetAnchor1();
                Automation.AddStructureChangedEventHandler(node, TreeScope.Children, structureChangedEventHandler);

                // Busca e invoca el botón "Iniciar sesión" que envía el formulario.
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
        /// Obtener en la ventana de login el nodo padre del formulario principal, desde el cual buscar los controles.
        /// </summary>
        /// <param name="setFocus">Si debe traer la ventana de login al primer plano.</param>
        private AutomationElement GetAnchor1(bool setFocus = false)
        {
            // Recupera la ventana de login.
            AutomationElement node = AutomationElement.RootElement;
            ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.NameProperty, "Wunderlist") });

            // Trae la ventana de login al primer plano.
            if (setFocus) ViewTree.Focus(node);

            // Busca el nodo de anclaje.
            return node;
        }

        /// <summary>
        /// Obtener en la ventana de login el nodo padre del formulario principal, desde el cual buscar los controles.
        /// </summary>
        /// <param name="setFocus">Si debe traer la ventana de login al primer plano.</param>
        private AutomationElement GetAnchor2(bool setFocus = false)
        {
            AutomationElement node = GetAnchor1(setFocus);
            ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.ClassNameProperty, "Windows.UI.Core.CoreWindow") });
            ViewTree.RetrieveChildNodePatternByCondition(ref node, new Condition[] { new PropertyCondition(AutomationElement.ClassNameProperty, "ScrollViewer") });
            return node;
        }

        /// <summary>
        /// Manejador para el evento de apertura de la ventana SEO WPF.
        /// </summary>
        /// <param name="src">Fuente.</param>
        /// <param name="e">Argumentos del evento.</param>
        private static void OnStructureChanged(object src, AutomationEventArgs e)
        {
            // Si la nueva ventana abierta corresponde a la aplicación SEO WPF, señala el controlador de enventos. En este controlador
            // está esperando el hilo principal, y cuando la señal le llega da el OK a la prueba funcional.
            if ((src as AutomationElement).Current.Name == "Wunderlist")
            {
                eventControl?.Set();
            }
        }
    }
}