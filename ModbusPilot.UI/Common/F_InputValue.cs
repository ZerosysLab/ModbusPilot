using System;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    public partial class F_InputValue : Form
    {
        public string InputValue { get; private set; }

        private Label lblName;
        private TextBox txtValue;
        private ComboBox cmbBool;
        private Button btnOk;
        private Button btnCancel;

        // 简单构造函数
        public F_InputValue(string pointName, string currentValue, string dataType)
        {
            InitializeComponent();
            InitializeCustomControls();

            this.Text = "写入数值";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new System.Drawing.Size(300, 180);

            lblName.Text = $"点位: {pointName}";

            // 根据数据类型判断显示什么控件
            bool isBool = dataType.ToLower().Contains("bool") || dataType.ToLower().Contains("coil");

            if (isBool)
            {
                txtValue.Visible = false;
                cmbBool.Visible = true;
                cmbBool.Items.Add("False (OFF)");
                cmbBool.Items.Add("True (ON)");

                // 尝试选中当前值
                bool cur = currentValue.ToLower() == "true" || currentValue == "1";
                cmbBool.SelectedIndex = cur ? 1 : 0;
            }
            else
            {
                txtValue.Visible = true;
                cmbBool.Visible = false;
                txtValue.Text = currentValue;
            }
        }

        private void InitializeCustomControls()
        {
            // 纯代码生成简单的 UI，免得你去拖控件
            lblName = new Label { Location = new System.Drawing.Point(20, 20), AutoSize = true, Font = new System.Drawing.Font("微软雅黑", 10F) };

            txtValue = new TextBox { Location = new System.Drawing.Point(20, 50), Width = 240, Font = new System.Drawing.Font("微软雅黑", 12F) };

            cmbBool = new ComboBox { Location = new System.Drawing.Point(20, 50), Width = 240, Font = new System.Drawing.Font("微软雅黑", 12F), DropDownStyle = ComboBoxStyle.DropDownList };

            btnOk = new Button { Text = "确定", Location = new System.Drawing.Point(100, 100), Width = 70, Height = 30, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "取消", Location = new System.Drawing.Point(190, 100), Width = 70, Height = 30, DialogResult = DialogResult.Cancel };

            this.Controls.Add(lblName);
            this.Controls.Add(txtValue);
            this.Controls.Add(cmbBool);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk; // 回车触发确定
            this.CancelButton = btnCancel; // ESC 触发取消

            btnOk.Click += BtnOk_Click;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (txtValue.Visible)
            {
                if (string.IsNullOrWhiteSpace(txtValue.Text))
                {
                    MessageBox.Show("数值不能为空");
                    return;
                }
                InputValue = txtValue.Text.Trim();
            }
            else
            {
                // 布尔值处理：选中索引1是True
                InputValue = (cmbBool.SelectedIndex == 1) ? "true" : "false";
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}