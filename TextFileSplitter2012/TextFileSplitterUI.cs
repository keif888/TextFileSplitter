using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Windows.Forms;

namespace Martin.SQLServer.Dts
{
    class TextFileSplitterUI : DataFlowComponentUI
    {
        #region Virtual methods

        /// <summary>
        /// Implementation of the method resposible for displaying the form.
        /// This one is abstract in the base class.
        /// </summary>
        /// <param name="parentControl">The owner window</param>
        /// <returns>true when the form is shown ok</returns>
        protected override bool EditImpl(IWin32Window parentControl)
        {
            using (TextFileSplitterForm form = new TextFileSplitterForm())
            {
                this.HookupEvents(form);

                if (form.ShowDialog(parentControl) == DialogResult.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion


        private void HookupEvents(TextFileSplitterForm form)
        {
            //form.GetInputColumns += new GetInputColumnsEventHandler(this.form_GetInputColumns);
            //form.SetInputColumn += new ChangeInputColumnEventHandler(this.form_SetInputColumn);
            //form.DeleteInputColumn += new ChangeInputColumnEventHandler(this.form_DeleteInputColumn);
            //form.GetOutputColumns += new GetOutputColumnsEventHandler(this.form_GetOutputColumns);
            //form.AddOutputColumn += new AddOutputColumnEventHandler(this.form_AddOutputColumn);
            //form.AlterOutputColumn += new AlterOutputColumnEventHandler(this.form_AlterOutputColumn);
            //form.DeleteOutputColumn += new DeleteOutputColumnEventHandler(this.form_DeleteOutputColumn);
            //form.CallErrorHandler += new ErrorEventHandler(this.form_CallErrorHandler);
        }
    }
}
