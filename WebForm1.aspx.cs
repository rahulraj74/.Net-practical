using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Practical4
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CreateTable();
            }
        }

        // Create DataTable and store it in ViewState
        private void CreateTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Name");
            dt.Columns.Add("Email");
            dt.Columns.Add("Phone");
            dt.Columns.Add("Stream");
            dt.Columns.Add("Enrollment");
            dt.Columns.Add("Gender");
            dt.Columns.Add("Age");
            dt.Columns.Add("Terms");

            ViewState["RegistrationData"] = dt;

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // Check all validators
            if (Page.IsValid)
            {
                DataTable dt = ViewState["RegistrationData"] as DataTable;

                if (dt == null)
                {
                    CreateTable();
                    dt = ViewState["RegistrationData"] as DataTable;
                }

                // Add entered data to table
                DataRow row = dt.NewRow();

                row["Name"] = TextBox1.Text;
                row["Email"] = TextBox2.Text;
                row["Phone"] = TextBox3.Text;
                row["Stream"] = DropDownList1.SelectedValue;
                row["Enrollment"] = TextBox4.Text;
                row["Gender"] = RadioButtonList1.SelectedValue;
                row["Age"] = TextBox5.Text;

                if (CheckBoxList1.SelectedIndex != -1)
                {
                    row["Terms"] = "Yes";
                }
                else
                {
                    row["Terms"] = "No";
                }

                dt.Rows.Add(row);

                // Save updated table
                ViewState["RegistrationData"] = dt;

                // Display data in GridView
                GridView1.DataSource = dt;
                GridView1.DataBind();

                // Clear form after submission
                TextBox1.Text = "";
                TextBox2.Text = "";
                TextBox3.Text = "";
                TextBox4.Text = "";
                TextBox5.Text = "";

                DropDownList1.SelectedIndex = 0;
                RadioButtonList1.ClearSelection();
                CheckBoxList1.ClearSelection();
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            // Reset form
            TextBox1.Text = "";
            TextBox2.Text = "";
            TextBox3.Text = "";
            TextBox4.Text = "";
            TextBox5.Text = "";

            DropDownList1.SelectedIndex = 0;
            RadioButtonList1.ClearSelection();
            CheckBoxList1.ClearSelection();
        }

        protected void CustomValidator1_ServerValidate(
            object source,
            ServerValidateEventArgs args)
        {
            args.IsValid = CheckBoxList1.SelectedIndex != -1;

            if (!args.IsValid)
            {
                CustomValidator1.ErrorMessage =
                    "You must agree to the Terms of Service and Privacy Policy.";
                CustomValidator1.ForeColor =
                    System.Drawing.Color.Red;
            }
        }
    }
}