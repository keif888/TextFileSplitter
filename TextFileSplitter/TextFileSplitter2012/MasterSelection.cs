using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Martin.SQLServer.Dts
{
    public partial class MasterSelection : Form
    {
        private Boolean _isLoading = false;
        public MasterSelection()
        {
            _isLoading = true;
            InitializeComponent();
            _isLoading = false;
        }

        public void initMasterList(List<String> masterNames)
        {
            _isLoading = true;
            btnOk.Enabled = false;
            lbMasters.Items.Clear();
            foreach (String name in masterNames)
            {
                lbMasters.Items.Add(name);
            }
        }

        public String SelectedMaster { get; private set; }

        private void lbMasters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isLoading)
            {
                if (lbMasters.Items.Count > 0)
                {
                    btnOk.Enabled = true;
                    SelectedMaster = (String)lbMasters.SelectedItem;
                }
            }
        }
    }
}
