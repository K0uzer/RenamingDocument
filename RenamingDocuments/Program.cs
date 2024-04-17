namespace RenamingDocuments;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    public static void Main()
    {
        FindOutThePresenceOfFolderInTheDirectory();
    }

    public static void FindOutThePresenceOfFolderInTheDirectory()
    {
        try
        {
            // Массивы //
            List<string> fileContentOfDlg = new List<string>();
            List<string> fileContentOfNotDlg = new List<string>();

            // Регулярные выражения //
            string isDlg = @"dlg";
            string clippingLastAll = @"_\d{1,9}_\d{1,9}.pdf";
            string clippingNumberForSortFile = @"_\d{1,9}.pdf";
            string clippingBeginningNotDlg = @"kvit_";
            string clippingBeginningDlg = @"kvit_dlg_";
            string clippingLast = @"_\d{1,9}";

            Console.WriteLine("Введите имя папки:");
            string folderName = Console.ReadLine();
            
            // Path //
            string dirMain = $"{Environment.CurrentDirectory}\\{folderName}";
            string dirNew = $"{Environment.CurrentDirectory}\\newDir";

            string[] oldFilesList = Directory.GetFiles(dirMain, "*.pdf");

            //// Создание папки //
            if (!Directory.Exists("newDir")) Directory.CreateDirectory("newDir");

            foreach (string fileName in oldFilesList)
            {
                Regex regexOfDlg = new Regex(isDlg);
                string elementOfArray = fileName.Replace($"{dirMain}", "");
                Match clippingDlg = regexOfDlg.Match(elementOfArray);
                RegexOptions optionsNotDlg = RegexOptions.Multiline;
                Regex regex = new Regex(clippingLastAll, optionsNotDlg);
                Regex regexSortFile = new Regex(clippingNumberForSortFile, optionsNotDlg);
                Regex regexSort = new Regex(clippingLast);

                if (clippingDlg.Value != "dlg")
                {
                    string numberForSortFile = regexSortFile.Replace(fileName, "").Replace(clippingBeginningNotDlg, "").Replace($"_{isDlg}", "").Replace(dirMain, "").Replace("\\", "");
                    string sortFiles = regexSort.Match(numberForSortFile).Value;
                    string counter = Convert.ToString(1000 - Convert.ToInt32(sortFiles.Replace($"_", "")));
                    string indexFile = regex.Replace(fileName, "").Replace(clippingBeginningNotDlg, "").Replace($"_{isDlg}", "").Replace(dirMain, "").Replace("\\", "");
                    fileContentOfNotDlg.Add($"{dirMain}\\{elementOfArray.Replace("\\", "")}");
                    fileContentOfNotDlg.Add($"{dirNew}\\{indexFile}_2_{counter}_{sortFiles}.{elementOfArray.Replace("\\", "")}");
                }
                else
                {
                    string numberForSortFile = regexSortFile.Replace(fileName, "").Replace(clippingBeginningDlg, "").Replace($"_{isDlg}", "").Replace(dirMain, "").Replace("\\", "");
                    string sortFiles = regexSort.Match(numberForSortFile).Value;
                    string counter = Convert.ToString(1000 - Convert.ToInt32(sortFiles.Replace($"_", "")));
                    string indexFile = regex.Replace(fileName, "").Replace(clippingBeginningDlg, "").Replace($"_{isDlg}", "").Replace(dirMain, "").Replace("\\", "");
                    fileContentOfDlg.Add($"{dirMain}\\{elementOfArray.Replace("\\", "")}");
                    fileContentOfDlg.Add($"{dirNew}\\{indexFile}_1_{counter}_{sortFiles}.{elementOfArray.Replace("\\", "")}");
                }
            }

            Console.WriteLine("Файлы dlg:");
            Console.WriteLine(fileContentOfDlg.Count / 2);
            Console.WriteLine("Файлы not dlg:");
            Console.WriteLine(fileContentOfNotDlg.Count / 2);
            Console.WriteLine("Всего:");
            Console.WriteLine((fileContentOfDlg.Count / 2) + (fileContentOfNotDlg.Count / 2));

            // Создаем новый поток
            Thread copyFileDlg = new (() => Dlg(fileContentOfDlg));
            Thread copyFileNotDlg = new (() => NotDlg(fileContentOfNotDlg));

            copyFileDlg.Start();
            copyFileNotDlg.Start();

            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    static void Dlg(List<string> fileContentOfDlg)
    {
        for (int i = 0; i < fileContentOfDlg.Count; i += 2) File.Copy($"{fileContentOfDlg[i]}", $"{fileContentOfDlg[i + 1]}");
    }

    static void NotDlg(List<string> fileContentOfNotDlg)
    {
        for (int i = 0; i < fileContentOfNotDlg.Count; i += 2) File.Copy($"{fileContentOfNotDlg[i]}", $"{fileContentOfNotDlg[i + 1]}");
    }
}
