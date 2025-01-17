﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.Form_DBManager.Form_Mits
{
    public partial class ClxhParamForm : Form
    {
        public ClxhParamForm()
        {
            List<string> fuelTypeList = Utils.GetFuelType("");
            InitializeComponent();
            this.panelControl.Location = new Point(0, 0);
            this.panelControl.Width = 1250;
            this.panelControl.Height = 1000;
            this.cbrllx.Text = fuelTypeList[0];
            this.tbjkqczjxs.Text = Utils.qymc;
            this.tc.SelectedTabPage = this.tp1;
            //comboBoxEdit1.
            //this.dropDownButton1

            string fuelTypeName = this.cbrllx.Text.Trim();
            if (fuelTypeName == "汽油" || fuelTypeName == "柴油" || fuelTypeName == "两用燃料" || fuelTypeName == "双燃料")
            {
                getParamList("传统能源");
            }
            else
            {
                getParamList(fuelTypeName);
            }
            this.cbrllx.Properties.Items.AddRange(fuelTypeList.ToArray());
        }

        // 燃料类型选中值变化时,显示不同燃料类型对应的子参数
        private void cbrllx_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.tc.SelectedTabPage = this.tp2;
                string strType = this.cbrllx.SelectedItem.ToString().Trim();
                if (strType != "" && strType != null)
                {
                    if (strType == "汽油" || strType == "柴油" || strType == "两用燃料" || strType == "双燃料")
                    {
                        getParamList("传统能源");
                    }
                    else
                    {
                        getParamList(strType);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取车型燃料参数失败：" + ex.Message);
            }
        }

        // 保存车型参数数据
        private void btnbaocun_Click(object sender, EventArgs e)
        {
            string msg = Utils.VerifyRLParam(this.tlp.Controls);
            try
            {
                if (!this.dxErrorProvider.HasErrors && string.IsNullOrEmpty(msg))
                {
                    this.SaveClxhParam();
                    MessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("请核对页面信息是否填写正确！" + msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 关闭本页面
        private void btnCon_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 保存SQL
        private void SaveClxhParam()
        {
            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);
            con.Open();
            OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务
            try
            {
                string strCreater = Utils.userId;

                #region 保存车型基本参数

                string basicParamName = @"QCSCQY,JKQCZJXS,CLXH,HGSPBM,CLZL,YYC,QDXS,ZWPS,ZCZBZL,ZDSJZZL,
                                    ZGCS,EDZK,LTGG,LJ,ZJ,RLLX,JYBGBH,JYJGMC,CREATETIME,UPDATETIME";
                StringBuilder basicParamValue = new StringBuilder();
                basicParamValue.Append("'" + this.tbqcscqy.Text.Trim());
                basicParamValue.Append("','" + this.tbjkqczjxs.Text.Trim());
                basicParamValue.Append("','" + this.tbclxh.Text.Trim());
                basicParamValue.Append("','" + this.tbHgspbm.Text.Trim());
                basicParamValue.Append("','" + this.cbclzl.Text.Trim());
                basicParamValue.Append("','" + this.cbyyc.Text.Trim());
                basicParamValue.Append("','" + this.cbqdxs.Text.Trim());
                basicParamValue.Append("','" + this.tbzwps.Text.Trim());
                basicParamValue.Append("','" + this.tbzczbzl.Text.Trim());
                basicParamValue.Append("','" + this.tbzdsjzzl.Text.Trim());
                basicParamValue.Append("','" + this.tbzgcs.Text.Trim());
                basicParamValue.Append("','" + this.tbedzk.Text.Trim());
                basicParamValue.Append("','" + this.tbltgg.Text.Trim());
                basicParamValue.Append("','" + this.tblj.Text.Trim());
                basicParamValue.Append("','" + this.tbzj.Text.Trim());
                basicParamValue.Append("','" + this.cbrllx.Text.Trim());
                basicParamValue.Append("','" + this.tbbgbh.Text.Trim());
                basicParamValue.Append("','" + this.tbjcjgmc.Text.Trim());
                basicParamValue.Append("',#" + Convert.ToDateTime(DateTime.Today, CultureInfo.InvariantCulture));
                basicParamValue.Append("#,#" + Convert.ToDateTime(DateTime.Today, CultureInfo.InvariantCulture));
                basicParamValue.Append("#");

                string fuelParamName = string.Empty;
                StringBuilder fuelParamValue = new StringBuilder();

                // 遍历子参数
                foreach (Control c in this.tlp.Controls)
                {
                    if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit)
                    {
                        //Control[] lblc = clj.Controls.Find("lbl" + c.Name, true);
                        fuelParamName += "," + Convert.ToString(c.Name).Trim();
                        fuelParamValue.Append("','" + Convert.ToString(c.Text).Trim());
                    }
                }
                fuelParamValue.Append("'");

                // 保存车辆基本信息
                string sqlStr = string.Format(@"INSERT INTO CTNY_MAIN
                                ({0}{1}) VALUES({2}{3})", basicParamName, fuelParamName, basicParamValue.ToString(), fuelParamValue.ToString().Substring(1));

                AccessHelper.ExecuteNonQuery(tra, sqlStr, null);
                #endregion
                tra.Commit();
            }
            catch (Exception ex)
            {
                tra.Rollback();
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        // 设置不可编辑
        private void setEnable()
        {
            foreach (Control c in this.tlp.Controls)
            {
                if (c is TextEdit || c is DevExpress.XtraEditors.TextEdit)
                {
                    c.Enabled = false;
                }
            }
            foreach (Control c in this.paneljiben.Controls)
            {
                if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit)
                {
                    c.Enabled = false;
                }
            }
        }

        // 显示油耗参数窗口
        public void getParamList(string strType)
        {
            // 先清空，再添加
            this.tlp.Controls.Clear();
            this.tlp.Location = new Point(10, 30);
            string sql = "SELECT PARAM_CODE, PARAM_NAME, FUEL_TYPE, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE FROM RLLX_PARAM WHERE   (FUEL_TYPE = '" + strType + "' AND STATUS = '1') ORDER BY ORDER_RULE";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                // textbox类型
                if (dr["CONTROL_TYPE"].ToString() == "TEXT")
                {
                    Label lbl = new Label();
                    lbl.Width = 160;
                    lbl.Height = 30;
                    lbl.Name = "lbl" + dr["PARAM_CODE"].ToString();
                    lbl.Text = dr["PARAM_NAME"].ToString();
                    lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                    TextEdit tb = new TextEdit();
                    tb.Width = 250;
                    tb.Height = 28;
                    tb.Name = dr["PARAM_CODE"].ToString();

                    Label lbll = new Label();
                    lbll.Width = 100;
                    lbll.Height = 30;
                    lbll.Text = dr["PARAM_REMARK"].ToString();
                    lbll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                    this.tlp.Controls.Add(lbl);
                    this.tlp.Controls.Add(tb);
                    this.tlp.Controls.Add(lbll);
                }
                // OPTION类型
                if (dr["CONTROL_TYPE"].ToString() == "OPTION")
                {
                    Label lbl = new Label();
                    lbl.Width = 160;
                    lbl.Height = 30;
                    lbl.Name = "lbl" + dr["PARAM_CODE"].ToString();
                    lbl.Text = dr["PARAM_NAME"].ToString();
                    lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                    DevExpress.XtraEditors.ComboBoxEdit cbe = new ComboBoxEdit();
                    cbe.Width = 250;
                    cbe.Height = 28;
                    cbe.Name = dr["PARAM_CODE"].ToString();
                    cbe.Properties.Items.AddRange(getArray(dr["CONTROL_VALUE"].ToString()));
                    cbe.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

                    Label lbll = new Label();
                    lbll.Width = 100;
                    lbll.Height = 30;
                    lbll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                    this.tlp.Controls.Add(lbl);
                    this.tlp.Controls.Add(cbe);
                    this.tlp.Controls.Add(lbll);
                }

                tlp.Location = new Point(0, 15);
            }

        }

        // 将字符串转换为数组
        public String[] getArray(string strValue)
        {
            String[] str = new String[] { };
            return strValue.Split('/');

        }

        #region 输入验证

        private void tbzwps_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbzwps.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbzwps, "");
            }
            else if (!IsInt(this.tbzwps.Text.Trim()))
            {
                this.dxErrorProvider.SetError(tbzwps, "应填写整数");
            }
            else
            {
                dxErrorProvider.SetError(tbzwps, "");
            }
        }

        private void tbzdsjzzl_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbzdsjzzl.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbzdsjzzl, "");
            }
            else if (!IsInt(this.tbzdsjzzl.Text.Trim()))
            {
                this.dxErrorProvider.SetError(tbzdsjzzl, "应填写整数");
            }
            else
            {
                dxErrorProvider.SetError(tbzdsjzzl, "");
            }
        }

        private void tbedzk_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbedzk.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbedzk, "");
            }
            else if (!IsInt(this.tbedzk.Text.Trim()))
            {
                this.dxErrorProvider.SetError(tbedzk, "应填写整数");
            }
            else
            {
                dxErrorProvider.SetError(tbedzk, "");
            }
        }

        private void tblj_Validating(object sender, CancelEventArgs e)
        {
            if (this.tblj.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tblj, "");
            }
            else if (!this.VerifyParamFormat(this.tblj.Text.Trim(), '/') || this.tblj.Text.Trim().IndexOf('/') < 0)
            {
                this.dxErrorProvider.SetError(tblj, "应填写整数，前后轮距，中间用”/”隔开");
            }
            else
            {
                dxErrorProvider.SetError(tblj, "");
            }
        }

        private void tbzj_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbzj.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbzj, "");
            }
            else if (!this.IsInt(this.tbzj.Text.Trim()))
            {
                this.dxErrorProvider.SetError(tbzj, "应填写整数");
            }
            else
            {
                dxErrorProvider.SetError(tbzj, "");
            }
        }

        private void tbzgcs_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbzgcs.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbzgcs, "");
            }
            else if (!this.VerifyParamFormat(this.tbzgcs.Text.Trim(), ','))
            {
                this.dxErrorProvider.SetError(tbzgcs, "应填写整数，多个数值应以半角“,”隔开，中间不留空格");
            }
            else
            {
                dxErrorProvider.SetError(tbzgcs, "");
            }
        }

        private void tbzczbzl_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbzczbzl.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbzczbzl, "");
            }
            else if (!this.VerifyParamFormat(this.tbzczbzl.Text.Trim(), ','))
            {
                this.dxErrorProvider.SetError(tbzczbzl, "应填写整数，多个数值应以半角“,”隔开，中间不留空格");
            }
            else
            {
                dxErrorProvider.SetError(tbzczbzl, "");
            }
        }

        private void tbltgg_Validating(object sender, CancelEventArgs e)
        {
            string ltgg = this.tbltgg.Text.Trim();
            if (ltgg == "")
            {
                this.dxErrorProvider.SetError(tbltgg, "");
            }
            else if (VerifyLtgg(ltgg))
            {
                this.dxErrorProvider.SetError(tbltgg, "前后轮距不相同以(前轮轮胎型号)/(后轮轮胎型号)(引号内为半角括号，且中间不留不必要的空格)");
            }
            else
            {
                dxErrorProvider.SetError(tbltgg, "");
            }
        }

        private bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+]?\d*$");
        }

        private bool VerifyLtgg(string ltgg)
        {
            bool flag = false;
            try
            {
                if (!string.IsNullOrEmpty(ltgg))
                {
                    int indexLtgg = ltgg.IndexOf(")/(");
                    if (indexLtgg > -1)
                    {
                        string ltggHead = ltgg.Substring(0, indexLtgg + 1);
                        string ltggEnd = ltgg.Substring(indexLtgg + 3);

                        if (!ltggHead.StartsWith("(") || !ltggEnd.EndsWith(")"))
                        {
                            flag = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return flag;
        }

        private bool VerifyParamFormat(string value, char c)
        {
            if (!string.IsNullOrEmpty(c.ToString()))
            {
                string[] valueArr = value.Split(c);
                if (valueArr[0] == "" || valueArr[valueArr.Length - 1] == "")
                {
                    return false;
                }
                foreach (string val in valueArr)
                {
                    if (!this.IsInt(val))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
