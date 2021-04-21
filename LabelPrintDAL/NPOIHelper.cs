using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI;
using FPLabelData.Entity;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Configuration;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Data;

namespace LabelPrintDAL
{
    public class NPOIHelper
    {
        /// <summary>
        /// 读取Excel表，返回物料备案集合
        /// </summary>
        /// <param name="sheetName">sheet名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public List<ExtractInventoryTool_Material> ExcelToMaterialList(string sheetName, bool isFirstRowColumn, string fileName, int clientOid, string clientUniqueCode, out string errorMessage)
        {
            errorMessage = string.Empty;
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<ExtractInventoryTool_Material> result = new List<ExtractInventoryTool_Material>();
            List<string> uniqueCodeList = new List<string>();
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                        workbook = new XSSFWorkbook(fs);
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
                        workbook = new HSSFWorkbook(fs);
                    if (string.IsNullOrEmpty(sheetName))
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    else
                    {
                        sheet = workbook.GetSheet(sheetName);
                        if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                        {
                            sheet = workbook.GetSheetAt(0);
                        }
                    }
                    if (sheet == null)
                        return result;
                    #region 读取Sheet
                    int rowNum = sheet.LastRowNum;
                    int firstRowNum = isFirstRowColumn ? sheet.FirstRowNum : sheet.FirstRowNum + 1;
                    for (int i = firstRowNum; i <= rowNum; i++)
                    {
                        //一行就是一条物料备案
                        IRow row = sheet.GetRow(i);
                        if (row == null)
                            continue;
                        if (row.GetCell(3) == null)
                            continue;
                        ExtractInventoryTool_Material material = new ExtractInventoryTool_Material();
                        material.SupplierCode = ConvertCellToString(row.GetCell(0));//供应商代码
                        material.Supplier = ConvertCellToString(row.GetCell(1));//供应商
                        material.Code = ConvertCellToString(row.GetCell(2));//零件号
                        material.Name = ConvertCellToString(row.GetCell(3));//零件名称
                        material.Client = clientOid;
                        material.UniqueCode = "c" + clientUniqueCode + "m" + material.Code + "s" + material.SupplierCode;
                        if (string.IsNullOrEmpty(material.Code))
                            continue;
                        if (uniqueCodeList.Contains(material.UniqueCode))
                        {
                            errorMessage= string.Format("零件号{0}存在重复值", material.Code);
                            return result;
                        }
                        uniqueCodeList.Add(material.UniqueCode);
                        result.Add(material);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 读取Excel表，返回BOM集合
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="client">客户信息</param>
        /// <returns></returns>
        public List<ExtractInventoryTool_BOM> ExcelToBOMList(string fileName, ExtractInventoryTool_Client client, out string errorMessage)
        {
            List<ExtractInventoryTool_BOM> result = new List<ExtractInventoryTool_BOM>();
            errorMessage = string.Empty;
            #region 读取客户配置
            string clientRuleConfig = ConfigurationManager.AppSettings[client.UniqueCode+"BOM"].ToString();
            ExtractInventoryTool_BOMImportRule clientRule = JsonConvert.DeserializeObject<ExtractInventoryTool_BOMImportRule>(clientRuleConfig);
            #endregion
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<string> uniqueCodeList = new List<string>();
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    #region 获取Excel
                    if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                        workbook = new XSSFWorkbook(fs);
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
                        workbook = new HSSFWorkbook(fs);
                    sheet = workbook.GetSheetAt(clientRule.St);
                    if (sheet == null)
                        return result; 
                    #endregion
                    #region 读取Excel
                    IRow vehicleCodeRow= sheet.GetRow(clientRule.VR);
                    #region 读取BOM的思路
                    // 每一行都是唯一一个物料，供应商代码和物料号作为唯一标识，这里做一个行列的双循环，在每一行中，有相同车型代码
                    // 的不同用量，先收集用量不为0的所有用量，然后按车型代码分组，取同种车型代码用量最大的用量，加入到最终结果中
                    // Meritar_Jeffrey	2021/04/05 02:25:25
                    #endregion
                    #region 获取所有当前客户的物料
                    DataTable dt = new ExtractInventoryTool_MaterialBLL().QueryMaterialByClientID(client.Oid.ToString());
                    List<ExtractInventoryTool_Material> materialList = new List<ExtractInventoryTool_Material>();
                    foreach (DataRow row in dt.Rows)
                    {
                        materialList.Add(new ExtractInventoryTool_Material()
                        {
                            Oid = Int32.Parse(row["Oid"].ToString()),
                            Code = row["Code"].ToString(),
                            SupplierCode=row["SupplierCode"].ToString(),
                            UniqueCode=row["UniqueCode"].ToString()
                        });
                    }
                    #endregion
                    for (int i = clientRule.MR; i <= sheet.LastRowNum; i++)//外循环--行
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null)
                            continue;
                        if (row.GetCell(clientRule.MC) == null)
                            continue;
                        string materialCode = ConvertCellToString(row.GetCell(clientRule.MC));
                        string supplierCode = ConvertCellToString(row.GetCell(clientRule.SC));
                        if (string.IsNullOrEmpty(materialCode))
                            continue;
                        List<ExtractInventoryTool_BOM> rowBomList = new List<ExtractInventoryTool_BOM>();
                        //判断物料是否已备案
                        ExtractInventoryTool_Material rowMaterial = materialList
                            .FirstOrDefault(m => m.Code.Trim().Equals(materialCode) &&
                                                            m.SupplierCode.Trim().Equals(supplierCode));
                        if (rowMaterial == null)
                        {
                            errorMessage = string.Format("第{0}行物料号{1}供应商代码{2}没有在系统备案", i + 1, materialCode, supplierCode);
                            return result;
                        }
                        bool isExist = false;
                        string uniqueCode = rowMaterial.UniqueCode;
                        //判断是否存在重复行
                        // 这里如果有重复行不再直接返回，而且新增bom和老bom取最大值
                        // Meritar_Jeffrey	2021/04/16 09:45:14
                        if (uniqueCodeList.Contains(uniqueCode))//这里判断一下有没有同种物料重复，如果有，直接返回，输出重复物料
                        {
                            //errorMessage = string.Format("第{0}行物料号{1}供应商代码{2}存在重复BOM记录", i + 1, materialCode, supplierCode);
                            //return result;
                            isExist = true;
                        }
                        else
                        {
                            uniqueCodeList.Add(uniqueCode);
                        }
                        for (int j = clientRule.VC; j <= row.LastCellNum - 3; j++)//内循环--列
                        {
                            ICell cell = row.GetCell(j);
                            if (cell == null || string.IsNullOrEmpty(cell.ToString()))
                                continue;
                            int unitUsage = 0;
                            unitUsage = Convert.ToInt32(cell.ToString());
                            if (unitUsage == 0)
                                continue;
                            ExtractInventoryTool_BOM bomCell = new ExtractInventoryTool_BOM();
                            bomCell.VehicleModelCode = Regex.Match(ConvertCellToString(vehicleCodeRow.GetCell(j)), client.RegexRule).Value;
                            bomCell.UnitUsage = unitUsage;
                            rowBomList.Add(bomCell);
                        }
                        IEnumerable<IGrouping<string, ExtractInventoryTool_BOM>> group = rowBomList.GroupBy(v => v.VehicleModelCode);
                        foreach (var groupItem in group)
                        {
                            ExtractInventoryTool_BOM bom = new ExtractInventoryTool_BOM();
                            bom.UnitUsage = groupItem.Max(v => v.UnitUsage);
                            bom.VehicleModelCode = groupItem.Key;
                            bom.Material = rowMaterial.Oid;
                            bom.UpdateTime = DateTime.Now;
                            bom.UniqueCode = uniqueCode + "v" + groupItem.Key;
                            if (isExist)
                            {
                                var existBom = result.FirstOrDefault(b => b.UniqueCode.Equals(bom.UniqueCode));
                                if (existBom == null)
                                {
                                    result.Add(bom);
                                }
                                else {
                                    existBom.UnitUsage = Math.Max(existBom.UnitUsage, bom.UnitUsage);
                                    existBom.UpdateTime = DateTime.Now;
                                }
                            }
                            else
                            {
                                result.Add(bom);
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 读取Excel表，返回ProductionPlan集合
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="client">客户信息</param>
        /// <returns></returns>
        public List<ExtractInventoryTool_ProductionPlan> ExcelToProductionPlanList(string fileName, ExtractInventoryTool_Client client, out string errorMessage)
        {
            List<ExtractInventoryTool_ProductionPlan> result = new List<ExtractInventoryTool_ProductionPlan>();
            errorMessage = string.Empty;
            #region 读取客户配置
            string clientRuleConfig = ConfigurationManager.AppSettings[client.UniqueCode+"PP"].ToString();
            ExtractInventoryTool_ProductionPlanImportRule clientRule = JsonConvert.DeserializeObject<ExtractInventoryTool_ProductionPlanImportRule>(clientRuleConfig);
            // Meritar_Jeffrey	2021/04/02 11:24:11
            #endregion
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<string> uniqueCodeList = new List<string>();
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    #region 获取Excel
                    if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                        workbook = new XSSFWorkbook(fs);
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
                        workbook = new HSSFWorkbook(fs);
                    sheet = workbook.GetSheetAt(clientRule.St);
                    if (sheet == null)
                        return result;
                    #endregion
                    #region 读取Excel
                    IRow dateRow = sheet.GetRow(clientRule.DR);
                    #region 读取生产计划的思路
                    #endregion
                    #region 获取所有BOM的车型代码
                    DataTable dt = new ExtractInventoryTool_BOMBLL().QueryVehicleModelCode();
                    List<string> vehicleCodeList = new List<string>();
                    foreach (DataRow row in dt.Rows)
                    {
                        vehicleCodeList.Add(row["VehicleModelCode"].ToString());
                    }
                    #endregion
                    for (int i = clientRule.VR; i <= sheet.LastRowNum; i++)//外循环--行
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null || row.GetCell(clientRule.VC) == null)
                            continue;
                        string vehicleModelCode = ConvertCellToString(row.GetCell(clientRule.VC));
                        if (string.IsNullOrEmpty(vehicleModelCode))
                            continue;
                        List<string> dateList = new List<string>();
                        //判断车型代码是否已备案
                        if (!vehicleCodeList.Contains(vehicleModelCode))
                        {
                            errorMessage = string.Format("第{0}行车型代码{1}没有在系统备案", i + 1, vehicleModelCode);
                            return result;
                        }
                        //判断是否存在重复行
                        if (uniqueCodeList.Contains(vehicleModelCode))//这里判断一下有没有同种车型代码重复，如果有，直接返回，输出重复物料
                        {
                            errorMessage = string.Format("第{0}行车型代码{1}存在重复记录", i + 1, vehicleModelCode);
                            return result;
                        }
                        uniqueCodeList.Add(vehicleModelCode);
                        for (int j = clientRule.UC; j <= row.LastCellNum - 2; j++)//内循环--列
                        {
                            if (dateRow.GetCell(j).CellType != CellType.Numeric)
                                break;
                            ICell cell = row.GetCell(j);
                            if (cell == null)
                                continue;
                            int unitNum = 0;
                            string unitNumStr = cell.ToString();
                            if (string.IsNullOrEmpty(unitNumStr))
                                continue;
                            unitNum = Convert.ToInt32(unitNumStr);
                            if (unitNum == 0)
                                continue;
                            string dateCell = dateRow.GetCell(j).DateCellValue.Date.ToString("yyyy-MM-dd");
                            if (dateList.Contains(dateCell))
                            {
                                errorMessage = string.Format("第{0}行第{1}和{2}列日期{3}存在重复记录", i + 1, j, j + 1, dateCell);
                                return result;
                            }
                            else
                            {
                                dateList.Add(dateCell);
                            }
                            ExtractInventoryTool_ProductionPlan ppCell = new ExtractInventoryTool_ProductionPlan();
                            ppCell.VehicleModelCode = vehicleModelCode;
                            //bomCell.ProductionDate = Regex.Match(dateRow.GetCell(j).ToString(), client.RegexRule).Value;
                            ppCell.ProductionDate = dateRow.GetCell(j).DateCellValue;
                            ppCell.UnitNum = unitNum;
                            ppCell.UpdateTime = DateTime.Now;
                            ppCell.Client = client.Oid;
                            ppCell.UniqueCode = "c" + client.UniqueCode + "v" + ppCell.VehicleModelCode + "d" + dateCell;
                            result.Add(ppCell);
                        }
                    }
                    #endregion
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 读取Excel表，返回库存集合
        /// </summary>
        /// <param name="sheetName">sheet名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public List<ExtractInventoryTool_Inventory> ExcelToInventoryList(string sheetName, string fileName, ExtractInventoryTool_Client client, out string errorMessage)
        {
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<ExtractInventoryTool_Inventory> result = new List<ExtractInventoryTool_Inventory>();
            errorMessage = string.Empty;
            List<string> uniqueCodeList = new List<string>();
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                        workbook = new XSSFWorkbook(fs);
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
                        workbook = new HSSFWorkbook(fs);
                    if (string.IsNullOrEmpty(sheetName))
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    else
                    {
                        sheet = workbook.GetSheet(sheetName);
                        if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                        {
                            sheet = workbook.GetSheetAt(0);
                        }
                    }
                    if (sheet == null)
                        return result;
                    #region 获取所有当前客户的物料
                    DataTable dt = new ExtractInventoryTool_MaterialBLL().QueryMaterialByClientID(client.Oid.ToString());
                    List<ExtractInventoryTool_Material> materialList = new List<ExtractInventoryTool_Material>();
                    foreach (DataRow row in dt.Rows)
                    {
                        materialList.Add(new ExtractInventoryTool_Material()
                        {
                            Oid = Int32.Parse(row["Oid"].ToString()),
                            Code = row["Code"].ToString(),
                            Name = row["Name"].ToString(),
                            SupplierCode = row["SupplierCode"].ToString(),
                            Supplier = row["Supplier"].ToString(),
                            UniqueCode = row["UniqueCode"].ToString()
                        });
                    }
                    #endregion
                    #region 读取Sheet
                    int rowNum = sheet.LastRowNum;
                    for (int i = 5; i <= rowNum; i++)
                    {
                        //一行就是一条库存信息
                        IRow row = sheet.GetRow(i);
                        if (row == null)
                            continue;
                        if (row.GetCell(1) == null)
                            continue;
                        string materialCode = ConvertCellToString(row.GetCell(1));
                        string supplierCode = ConvertCellToString(row.GetCell(3));


                        //判断物料是否已备案
                        ExtractInventoryTool_Material rowMaterial = materialList
                            .FirstOrDefault(m => m.Code.Trim().Equals(materialCode) &&
                                                            m.SupplierCode.Trim().Equals(supplierCode));
                        if (rowMaterial == null)
                        {
                            errorMessage = string.Format("第{0}行物料号{1}供应商代码{2}没有在系统备案", i + 1, materialCode, supplierCode);
                            return result;
                        }
                        string uniqueCode = rowMaterial.UniqueCode;
                        //判断是否存在重复行
                        if (uniqueCodeList.Contains(uniqueCode))//这里判断一下有没有同种物料重复，如果有，直接返回，输出重复物料
                        {
                            errorMessage = string.Format("第{0}行物料号{1}供应商代码{2}存在重复库存记录", i + 1, materialCode, supplierCode);
                            return result;
                        }
                        uniqueCodeList.Add(uniqueCode);
                        ExtractInventoryTool_Inventory inventory = new ExtractInventoryTool_Inventory();
                        inventory.SysInventory = Convert.ToInt32(ConvertCellToString(row.GetCell(5), "0"));//系统库存
                        inventory.Min = Convert.ToInt32(ConvertCellToString(row.GetCell(6), "0"));//MIN
                        inventory.Max = Convert.ToInt32(ConvertCellToString(row.GetCell(7), "0"));//MAX
                        inventory.HUB = Convert.ToInt32(ConvertCellToString(row.GetCell(8), "0"));//HUB库存
                        inventory.InTransit = Convert.ToInt32(ConvertCellToString(row.GetCell(9), "0"));//在途库存
                        inventory.Total = Convert.ToInt32(ConvertCellToString(row.GetCell(10), "0"));//总库存
                        inventory.Material = rowMaterial.Oid;
                        result.Add(inventory);
                    }
                    #endregion
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 单元格Cell判空
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <param name="defaultStr">默认值</param>
        /// <returns></returns>
        public string ConvertCellToString(ICell cell,string defaultStr="")
        {
            if (cell == null)
                return defaultStr;
            if (cell.CellType == CellType.Formula)
            {
                cell.SetCellType(CellType.String);
            }
            return cell.ToString().Trim();
        }

        public void WriteToPickUpDemandExcel(string tempPath, string newExcelPath, List<ExtractInventoryTool_PickUpDemandDetails> demandList)
        {
            IWorkbook workbook = null;
            ISheet sheet = null;
            using (FileStream fs = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new XSSFWorkbook(fs);
            }
            sheet = workbook.GetSheetAt(0);
            try
            {
                List<DateTime> productionDateList = demandList.Select(d => d.ProductionDate).Distinct().OrderBy(d => d).ToList();
                List<KeyValuePair<DateTime, int>> productionDateColumnList = new List<KeyValuePair<DateTime, int>>();
                IRow headerRow = sheet.GetRow(0);
                int productionDateColumnNum = 9;//列名行，时间列的起始列
                foreach (DateTime productionDate in productionDateList)
                {
                    headerRow.CreateCell(productionDateColumnNum).SetCellValue(productionDate.GetDateTimeFormats('M')[0].ToString());
                    productionDateColumnList.Add(new KeyValuePair<DateTime, int>(productionDate, productionDateColumnNum++));
                }
                headerRow.CreateCell(productionDateColumnNum).SetCellValue("总需求");
                var groupList = demandList.GroupBy(d => d.Code);
                int materilRowNum = 1;//物料行
                foreach (var group in groupList)
                {
                    var groupAsc = group.OrderBy(d => d.ProductionDate).ToList();
                    var first = groupAsc.First();
                    int totalUsage = 0;
                    IRow materialRow = sheet.CreateRow(materilRowNum++);
                    materialRow.CreateCell(0).SetCellValue(first.SupplierCode);
                    materialRow.CreateCell(1).SetCellValue(first.Supplier);
                    materialRow.CreateCell(2).SetCellValue(first.Code);
                    materialRow.CreateCell(3).SetCellValue(first.Name);
                    materialRow.CreateCell(4).SetCellValue(first.SysInventory);
                    materialRow.CreateCell(5).SetCellValue(first.InTransit);
                    materialRow.CreateCell(6).SetCellValue(first.HUB);
                    materialRow.CreateCell(7).SetCellValue(first.Total);
                    foreach (var item in groupAsc)
                    {
                        var DateColum = productionDateColumnList.FirstOrDefault(d => d.Key.Date.Equals(item.ProductionDate.Date));
                        materialRow.CreateCell(DateColum.Value).SetCellValue(item.DailyUsage);
                        totalUsage += item.DailyUsage;
                    }
                    materialRow.CreateCell(productionDateColumnNum).SetCellValue(totalUsage);
                }
                using (FileStream sw = File.Create(newExcelPath))
                {
                    workbook.Write(sw);
                }
                workbook.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
