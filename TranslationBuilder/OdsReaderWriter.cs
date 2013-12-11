using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using Ionic.Zip;

namespace OdsReadWrite
{
    internal sealed class OdsReaderWriter
    {
        // Namespaces. We need this to initialize XmlNamespaceManager so that we can search XmlDocument.
        private static string[,] namespaces = new string[,] 
        {
            {"table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0"},
            {"office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0"},
            {"style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0"},
            {"text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0"},            
            {"draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0"},
            {"fo", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0"},
            {"dc", "http://purl.org/dc/elements/1.1/"},
            {"meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0"},
            {"number", "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0"},
            {"presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0"},
            {"svg", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0"},
            {"chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0"},
            {"dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0"},
            {"math", "http://www.w3.org/1998/Math/MathML"},
            {"form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0"},
            {"script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0"},
            {"ooo", "http://openoffice.org/2004/office"},
            {"ooow", "http://openoffice.org/2004/writer"},
            {"oooc", "http://openoffice.org/2004/calc"},
            {"dom", "http://www.w3.org/2001/xml-events"},
            {"xforms", "http://www.w3.org/2002/xforms"},
            {"xsd", "http://www.w3.org/2001/XMLSchema"},
            {"xsi", "http://www.w3.org/2001/XMLSchema-instance"},
            {"rpt", "http://openoffice.org/2005/report"},
            {"of", "urn:oasis:names:tc:opendocument:xmlns:of:1.2"},
            {"rdfa", "http://docs.oasis-open.org/opendocument/meta/rdfa#"},
            {"config", "urn:oasis:names:tc:opendocument:xmlns:config:1.0"}
        };

        // Read zip stream (.ods file is zip file).
        private ZipFile GetZipFile(Stream stream)
        {
            return ZipFile.Read(stream);
        }

        // Read zip file (.ods file is zip file).
        private ZipFile GetZipFile(string inputFilePath)
        {
            return ZipFile.Read(inputFilePath);
        }

        private XmlDocument GetContentXmlFile(ZipFile zipFile)
        {
            // Get file(in zip archive) that contains data ("content.xml").
            ZipEntry contentZipEntry = zipFile["content.xml"];

            // Extract that file to MemoryStream.
            Stream contentStream = new MemoryStream();
            contentZipEntry.Extract(contentStream);
            contentStream.Seek(0, SeekOrigin.Begin);

            // Create XmlDocument from MemoryStream (MemoryStream contains content.xml).
            XmlDocument contentXml = new XmlDocument();
            contentXml.Load(contentStream);

            return contentXml;
        }

        private XmlNamespaceManager InitializeXmlNamespaceManager(XmlDocument xmlDocument)
        {
            XmlNamespaceManager nmsManager = new XmlNamespaceManager(xmlDocument.NameTable);

            for (int i = 0; i < namespaces.GetLength(0); i++)
                nmsManager.AddNamespace(namespaces[i, 0], namespaces[i, 1]);

            return nmsManager;
        }

        /// <summary>
        /// Read .ods file and store it in DataSet.
        /// </summary>
        /// <param name="inputFilePath">Path to the .ods file.</param>
        /// <returns>DataSet that represents .ods file.</returns>
        public DataSet ReadOdsFile(string inputFilePath)
        {
            ZipFile odsZipFile = this.GetZipFile(inputFilePath);

            // Get content.xml file
            XmlDocument contentXml = this.GetContentXmlFile(odsZipFile);

            // Initialize XmlNamespaceManager
            XmlNamespaceManager nmsManager = this.InitializeXmlNamespaceManager(contentXml);

            DataSet odsFile = new DataSet(Path.GetFileName(inputFilePath));

            foreach (XmlNode tableNode in this.GetTableNodes(contentXml, nmsManager))
                odsFile.Tables.Add(this.GetSheet(tableNode, nmsManager));

            return odsFile;
        }

        // In ODF sheet is stored in table:table node
        private XmlNodeList GetTableNodes(XmlDocument contentXmlDocument, XmlNamespaceManager nmsManager)
        {
            return contentXmlDocument.SelectNodes("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager);
        }

        private DataTable GetSheet(XmlNode tableNode, XmlNamespaceManager nmsManager)
        {
            DataTable sheet = new DataTable(tableNode.Attributes["table:name"].Value);

            XmlNodeList rowNodes = tableNode.SelectNodes("table:table-row", nmsManager);

            int rowIndex = 0;
            foreach (XmlNode rowNode in rowNodes)
                this.GetRow(rowNode, sheet, nmsManager, ref rowIndex);

            return sheet;
        }

        private void GetRow(XmlNode rowNode, DataTable sheet, XmlNamespaceManager nmsManager, ref int rowIndex)
        {
            XmlAttribute rowsRepeated = rowNode.Attributes["table:number-rows-repeated"];
            if (rowsRepeated == null || Convert.ToInt32(rowsRepeated.Value, CultureInfo.InvariantCulture) == 1)
            {
                while (sheet.Rows.Count < rowIndex)
                    sheet.Rows.Add(sheet.NewRow());

                DataRow row = sheet.NewRow();

                XmlNodeList cellNodes = rowNode.SelectNodes("table:table-cell", nmsManager);

                int cellIndex = 0;
                foreach (XmlNode cellNode in cellNodes)
                    this.GetCell(cellNode, row, nmsManager, ref cellIndex);

                sheet.Rows.Add(row);

                rowIndex++;
            }
            else
            {
                rowIndex += Convert.ToInt32(rowsRepeated.Value, CultureInfo.InvariantCulture);
            }

            // sheet must have at least one cell
            if (sheet.Rows.Count == 0)
            {
                sheet.Rows.Add(sheet.NewRow());
                sheet.Columns.Add();
            }
        }

        private void GetCell(XmlNode cellNode, DataRow row, XmlNamespaceManager nmsManager, ref int cellIndex)
        {
            XmlAttribute cellRepeated = cellNode.Attributes["table:number-columns-repeated"];
            string cellValue = this.ReadCellValue(cellNode);

            int repeats = 1;
            if (cellRepeated != null)
            {
                repeats = Convert.ToInt32(cellRepeated.Value, CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrEmpty(cellValue))
            {
                for (int i = 0; i < repeats; i++)
                {
                    DataTable sheet = row.Table;

                    while (sheet.Columns.Count <= cellIndex)
                        sheet.Columns.Add();

                    row[cellIndex] = cellValue;

                    cellIndex++;
                }
            }
            else
            {
                cellIndex += repeats;
            }
        }

        private string ReadCellValue(XmlNode cell)
        {
            XmlAttribute cellVal = cell.Attributes["office:value"];

            if (cellVal == null)
                return String.IsNullOrEmpty(cell.InnerText) ? null : cell.InnerText;
            else
                return cellVal.Value;
        }

        /// <summary>
        /// Writes DataSet as .ods file.
        /// </summary>
        /// <param name="odsFile">DataSet that represent .ods file.</param>
        /// <param name="outputFilePath">The name of the file to save to.</param>
        public void WriteOdsFile(DataSet odsFile, string outputFilePath)
        {
            ZipFile templateFile = this.GetZipFile(Assembly.GetExecutingAssembly().GetManifestResourceStream("OdsReadWrite.template.ods"));

            XmlDocument contentXml = this.GetContentXmlFile(templateFile);

            XmlNamespaceManager nmsManager = this.InitializeXmlNamespaceManager(contentXml);

            XmlNode sheetsRootNode = this.GetSheetsRootNodeAndRemoveChildrens(contentXml, nmsManager);

            foreach (DataTable sheet in odsFile.Tables)
                this.SaveSheet(sheet, sheetsRootNode);

            this.SaveContentXml(templateFile, contentXml);

            templateFile.Save(outputFilePath);
        }

        private XmlNode GetSheetsRootNodeAndRemoveChildrens(XmlDocument contentXml, XmlNamespaceManager nmsManager)
        {
            XmlNodeList tableNodes = this.GetTableNodes(contentXml, nmsManager);

            XmlNode sheetsRootNode = tableNodes.Item(0).ParentNode;
            // remove sheets from template file
            foreach (XmlNode tableNode in tableNodes)
                sheetsRootNode.RemoveChild(tableNode);

            return sheetsRootNode;
        }

        private void SaveSheet(DataTable sheet, XmlNode sheetsRootNode)
        {
            XmlDocument ownerDocument = sheetsRootNode.OwnerDocument;

            XmlNode sheetNode = ownerDocument.CreateElement("table:table", this.GetNamespaceUri("table"));

            XmlAttribute sheetName = ownerDocument.CreateAttribute("table:name", this.GetNamespaceUri("table"));
            sheetName.Value = sheet.TableName;
            sheetNode.Attributes.Append(sheetName);

            this.SaveColumnDefinition(sheet, sheetNode, ownerDocument);

            this.SaveRows(sheet, sheetNode, ownerDocument);

            sheetsRootNode.AppendChild(sheetNode);
        }

        private void SaveColumnDefinition(DataTable sheet, XmlNode sheetNode, XmlDocument ownerDocument)
        {
            XmlNode columnDefinition = ownerDocument.CreateElement("table:table-column", this.GetNamespaceUri("table"));

            XmlAttribute columnsCount = ownerDocument.CreateAttribute("table:number-columns-repeated", this.GetNamespaceUri("table"));
            columnsCount.Value = sheet.Columns.Count.ToString(CultureInfo.InvariantCulture);
            columnDefinition.Attributes.Append(columnsCount);

            sheetNode.AppendChild(columnDefinition);
        }

        private void SaveRows(DataTable sheet, XmlNode sheetNode, XmlDocument ownerDocument)
        {
            DataRowCollection rows = sheet.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                XmlNode rowNode = ownerDocument.CreateElement("table:table-row", this.GetNamespaceUri("table"));

                this.SaveCell(rows[i], rowNode, ownerDocument);

                sheetNode.AppendChild(rowNode);
            }
        }

        private void SaveCell(DataRow row, XmlNode rowNode, XmlDocument ownerDocument)
        {
            object[] cells = row.ItemArray;

            for (int i = 0; i < cells.Length; i++)
            {
                XmlElement cellNode = ownerDocument.CreateElement("table:table-cell", this.GetNamespaceUri("table"));

                if (row[i] != DBNull.Value)
                {
                    // We save values as text (string)
                    XmlAttribute valueType = ownerDocument.CreateAttribute("office:value-type", this.GetNamespaceUri("office"));
                    valueType.Value = "string";
                    cellNode.Attributes.Append(valueType);

                    XmlElement cellValue = ownerDocument.CreateElement("text:p", this.GetNamespaceUri("text"));
                    cellValue.InnerText = row[i].ToString();
                    cellNode.AppendChild(cellValue);
                }

                rowNode.AppendChild(cellNode);
            }
        }

        private void SaveContentXml(ZipFile templateFile, XmlDocument contentXml)
        {
            templateFile.RemoveEntry("content.xml");

            MemoryStream memStream = new MemoryStream();
            contentXml.Save(memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            templateFile.AddEntry("content.xml", memStream);
        }

        private string GetNamespaceUri(string prefix)
        {
            for (int i = 0; i < namespaces.GetLength(0); i++)
            {
                if (namespaces[i, 0] == prefix)
                    return namespaces[i, 1];
            }

            throw new InvalidOperationException("Can't find that namespace URI");
        }
    }
}
