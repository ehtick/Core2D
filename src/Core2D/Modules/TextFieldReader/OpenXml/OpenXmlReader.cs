﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Core2D.Modules.TextFieldReader.OpenXml;

public sealed class OpenXmlReader : ITextFieldReader<DatabaseViewModel>
{
    private readonly IServiceProvider? _serviceProvider;

    public OpenXmlReader(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name => "Xlsx (OpenXml)";

    public string Extension => "xlsx";

    private static string? ToString(Cell c, SharedStringTablePart? stringTable)
    {
        if (c.DataType is null)
        {
            return c.CellValue?.Text;
        }

        if (c.DataType.Value == CellValues.SharedString)
        {
            if (stringTable is not null)
            {
                int index = int.Parse(c.InnerText);
                var value = stringTable.SharedStringTable.ElementAt(index).InnerText;
                return value;
            }
        }
        else if (c.DataType.Value == CellValues.Boolean)
            return c.InnerText switch
            {
                "0" => "FALSE",
                _ => "TRUE",
            };
        else if (c.DataType.Value == CellValues.Number)
            return c.InnerText;
        else if (c.DataType.Value == CellValues.Error)
            return c.InnerText;
        else if (c.DataType.Value == CellValues.String)
            return c.InnerText;
        else if (c.DataType.Value == CellValues.InlineString)
            return c.InnerText;
        else if (c.DataType.Value == CellValues.Date) 
            return c.InnerText;

        return null;
    }

    private static IEnumerable<string?[]>? ReadFields(Stream stream)
    {
        var spreadsheetDocument = SpreadsheetDocument.Open(stream, false);

        var workbookPart = spreadsheetDocument.WorkbookPart;
        if (workbookPart is null)
        {
            yield break;
        }

        var worksheetPart = workbookPart.WorksheetParts.First();

        var sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

        var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

        foreach (var row in sheetData.Elements<Row>())
        {
            var fields = row.Elements<Cell>().Select(c => ToString(c, stringTable)).ToArray();
            yield return fields;
        }

        spreadsheetDocument.Dispose();
    }

    public DatabaseViewModel? Read(Stream stream)
    {
        var fields = ReadFields(stream)?.ToList();

        var name = "Db";

        if (stream is FileStream fileStream)
        {
            name = Path.GetFileNameWithoutExtension(fileStream.Name);
        }

        if (fields is null)
        {
            return null;
        }

        return _serviceProvider.GetService<IViewModelFactory>()?.FromFields(name, fields);
    }
}
