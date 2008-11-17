using System.Windows.Forms;

namespace NrkBrowser
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

        public Label LabelVersionVerdi
        {
            get { return labelVersionVerdi; }
            set { labelVersionVerdi = value; }
        }
    }
}