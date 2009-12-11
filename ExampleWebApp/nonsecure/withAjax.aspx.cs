using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CasDotNetExampleWebapp.nonsecure
{
  public partial class withAjax : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {}

    protected void Button1_Click(object sender, EventArgs e)
    {
      DateTime currentTime = DateTime.Now;
      Label1.Text = string.Format("You clicked the button at {0:F}!",
                                   currentTime);
    }
  }
}
