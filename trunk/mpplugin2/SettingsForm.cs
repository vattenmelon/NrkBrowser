using System.Windows.Forms;

namespace Vattenmelon.Nrk.Browser
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        public TextBox NameTextbox
        {
            get { return nameTextbox; }
            set { nameTextbox = value; }
        }

        public Label LabelVersionPluginVerdi
        {
            get { return labelVersionPluginVerdi; }
            set { labelVersionPluginVerdi = value; }
        }
        public Label LabelVersionLibraryVerdi
        {
            get { return labelVersionLibraryVerdi; }
            set { labelVersionLibraryVerdi = value; }
        }
    }
}