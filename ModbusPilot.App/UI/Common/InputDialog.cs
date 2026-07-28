using System;
using System.Drawing;
using System.Windows.Forms;

public static class InputDialog
{
    public static string Show(string prompt, string title, string defaultValue = "")
    {
        Form promptForm = new Form()
        {
            Width = 350,
            Height = 180,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = title,
            StartPosition = FormStartPosition.CenterScreen,
            MaximizeBox = false,
            MinimizeBox = false
        };

        Label textLabel = new Label() { Left = 20, Top = 20, Text = prompt, AutoSize = true };
        TextBox textBox = new TextBox() { Left = 20, Top = 60, Width = 290, Text = defaultValue };
        Button confirmation = new Button() { Text = "确定", Left = 190, Width = 120, Top = 95, DialogResult = DialogResult.OK };

        // 设为默认按钮，支持回车
        promptForm.AcceptButton = confirmation;

        promptForm.Controls.Add(textLabel);
        promptForm.Controls.Add(textBox);
        promptForm.Controls.Add(confirmation);

        return promptForm.ShowDialog() == DialogResult.OK ? textBox.Text : "";
    }
}