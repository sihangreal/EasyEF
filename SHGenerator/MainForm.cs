using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SHGenerator
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        private SqlHelper _sqlHelper;
        DataTable _targtDt = new DataTable();
        DataTable _tableNameDt = new DataTable();

        public MainForm()
        {
            InitializeComponent();
            InitEvents();
        }

        #region 方法
        private void InitEvents()
        {
            this.Load += MainForm_Load;
            this.editTableName.EditValueChanged += BarEditItem1_EditValueChanged;
            this.gridView1.MouseUp += GridView1_MouseUp;
            this.btnAdd.ItemClick += BtnAdd_ItemClick;

            this.btnGenerator1.ItemClick += BtnGenerator1_ItemClick;
        }

        private void InitDataTable()
        {
            DataColumn column = new DataColumn("字段名");
            DataColumn column2 = new DataColumn("字段类型");
            DataColumn column3 = new DataColumn("可否为空");
            DataColumn column4 = new DataColumn("是否主键");
            DataColumn column5 = new DataColumn("自动增长");
            _targtDt.Columns.AddRange(new DataColumn[] { column, column2, column3, column4, column5 });
            this.gridControl2.DataSource = _targtDt;
        }
        #endregion

        #region 事件
        private void MainForm_Load(object sender, EventArgs e)
        {
            _sqlHelper = SqlHelper.GetSqlHelper();

            _tableNameDt = _sqlHelper.GetAllTableNames();
            repositoryItemGridLookUpEdit1.BeginUpdate();
            repositoryItemGridLookUpEdit1.DataSource = _tableNameDt;  //数据源
            repositoryItemGridLookUpEdit1.ValueMember = "TABLE_NAME";
            repositoryItemGridLookUpEdit1.DisplayMember = "TABLE_NAME";
            repositoryItemGridLookUpEdit1.PopulateViewColumns();
            repositoryItemGridLookUpEdit1.View.Columns["TABLE_NAME"].Caption = "数据库表名";
            repositoryItemGridLookUpEdit1.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            repositoryItemGridLookUpEdit1.View.BestFitColumns();//自动调整所有列的长度
            repositoryItemGridLookUpEdit1.ShowFooter = true;
            repositoryItemGridLookUpEdit1.View.OptionsView.ShowAutoFilterRow = false; //显示不显示grid上第一个空行,也是用于检索的应用
            repositoryItemGridLookUpEdit1.AutoComplete = false;
            repositoryItemGridLookUpEdit1.ImmediatePopup = true;
            repositoryItemGridLookUpEdit1.PopupFilterMode = PopupFilterMode.Contains;
            repositoryItemGridLookUpEdit1.TextEditStyle = TextEditStyles.Standard; //配置,用于像文本框那样呀,可自己录入,选择,些处是枚举,可自行设置.
            repositoryItemGridLookUpEdit1.EndUpdate();

            this.gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 40;
            InitDataTable();
        }

        private void BarEditItem1_EditValueChanged(object sender, EventArgs e)
        {
            DataTable dt = _sqlHelper.GetTableInfo(this.editTableName.EditValue.ToString());
            this.gridControl1.DataSource = dt;
            this.gridControl1.RefreshDataSource();
        }

        private void GridView1_MouseUp(object sender, MouseEventArgs e)
        {
            GridHitInfo gridHitInfo = gridView1.CalcHitInfo(e.Location);
            if (gridHitInfo.InRow && e.Button == MouseButtons.Right)
            {
                gridView1.SelectRow(gridView1.FocusedRowHandle);
                this.popupMenu1.ShowPopup(Control.MousePosition);
            }
        }
        private void BtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int[] rows = gridView1.GetSelectedRows();
            foreach (int i in rows)
            {
                DataRow dr = this.gridView1.GetDataRow(i);
                _targtDt.Rows.Add(dr.ItemArray);
            }
            _targtDt.TableName = this.editTableName.EditValue.ToString();
        }


        private void BtnGenerator1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string calssName = this.editClassName.EditValue ==null? this.editTableName.EditValue.ToString() : this.editClassName.EditValue.ToString();
            Generator.GenerateSqlScript(calssName, _targtDt, ref sb);
            EditForm form3 = new EditForm();
            form3.SetText(sb.ToString());
            form3.Show();
        }

        #endregion
    }
}
