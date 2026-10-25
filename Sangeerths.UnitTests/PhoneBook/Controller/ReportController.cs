using PhoneBook.Services;
using PhoneBook.UI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneBook.Controller
{
    public class ReportController 
    {
        private readonly ReportService _reportService;
        private readonly ContactController _contactController;
        private readonly ConsoleUI _consoleUI;
        private enum ReportType
        {
            Pdf,
            Excel
        }
        public ReportController()
        {
            _reportService = new ReportService();
            _contactController = new ContactController();
            _consoleUI = new ConsoleUI();
        }
        public async Task ExportReportFlowAsync()
        {
            var contacts = await _contactController.ViewAllContacts();
            if (contacts.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No contacts to export.[/]");
                _consoleUI.Pause();
                return;
            }

            var reportType = AnsiConsole.Prompt(
                new SelectionPrompt<ReportType>()
                    .Title("Which [green]report format[/] would you like?")
                    .AddChoices(ReportType.Pdf, ReportType.Excel));

            string path;
            switch (reportType)
            {
                case ReportType.Pdf:
                    path = _reportService.GenerateContactsReport(contacts);
                    break;
                case ReportType.Excel:
                    path = _reportService.GenerateContactsExcelReport(contacts);
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Unsupported report type.[/]");
                    _consoleUI.Pause();
                    return;
            }

            AnsiConsole.MarkupLine($"[green]Report saved to:[/] {Path.GetFullPath(path)}");
            _consoleUI.Pause();
        }
    }
}