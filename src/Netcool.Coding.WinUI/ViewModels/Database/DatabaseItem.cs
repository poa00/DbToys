﻿using Netcool.Coding.Core.Database;

namespace Netcool.Coding.WinUI.ViewModels.Database;

public class DatabaseItem : TreeItem
{

    private readonly ISchemaReader _schemaReader;

    public List<Table> Tables { get; set; }

    // public ReactiveCommand<Unit, Unit> RefreshCommand { get; set; }
    // public ReactiveCommand<Unit, Unit> ExportToExcelCommand { get; set; }

    public DatabaseItem(string name, ISchemaReader schemaReader) : base(name, true)
    {
        Name = name;
        _schemaReader = schemaReader;
        // RefreshCommand = ReactiveCommand.Create(LoadChildren);
        // ExportToExcelCommand = ReactiveCommand.CreateFromTask(ExportToExcel);
    }

    protected override void LoadChildren()
    {
        var tables = _schemaReader.ReadTables(Name).OrderBy(t => t.DisplayName);
        Tables = tables.ToList();
        Children?.Clear();
        foreach (var table in Tables)
        {
            AddChild(new TableItem(table) { IsExpanded = true });
        }
    }
}
