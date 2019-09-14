using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Csv;
using Microsoft.Azure; // Namespace for Azure Configuration Manager
using Microsoft.Azure.Storage; // Namespace for Storage Client Library
using Microsoft.Azure.Storage.File; // Namespace for Azure Files

namespace TaskListApplication.Models
{
    public class TasklistCSVRemote : ITaskList
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudFileClient fileClient; 
        private readonly CloudFileShare share;
        private readonly CloudFileDirectory root;
        private readonly CloudFile dbFile;
        private readonly byte[] hdrBytes;

        private CsvOptions csvOptions = new CsvOptions // Defaults
        {
            RowsToSkip = 0, // Allows skipping of initial rows without csv data
            SkipRow = (row, idx) => string.IsNullOrEmpty(row) || row[0] == '#',
            Separator = '\0', // Autodetects based on first row
            TrimData = false, // Can be used to trim each cell
            Comparer = null, // Can be used for case-insensitive comparison for names
            HeaderMode = HeaderMode.HeaderPresent, // Assumes first row is a header row
            ValidateColumnCount = false, // Checks each row immediately for column count
            ReturnEmptyForMissingColumn = false, // Allows for accessing invalid column names
            Aliases = null, // A collection of alternative column names
            AllowNewLineInEnclosedFieldValues = false, // Respects new line (either \r\n or \n) characters inside field values enclosed in double quotes.
            AllowBackSlashToEscapeQuote = false, // Allows the sequence "\"" to be a valid quoted value (in addition to the standard """")
            AllowSingleQuoteToEncloseFieldValues = false, // Allows the single-quote character to be used to enclose field values
            NewLine = Environment.NewLine // The new line string to use when multiline field values are read (Requires "AllowNewLineInEnclosedFieldValues" to be set to "true" for this to have any effect.)
        };

        public TasklistCSVRemote() {
            this.storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));
            this.fileClient = storageAccount.CreateCloudFileClient();
            this.share = fileClient.GetShareReference("tasklistdatabasestore");
            this.root = this.share.GetRootDirectoryReference();
            // Get a reference to the file we created previously.
            dbFile = this.root.GetFileReference("tasklist.csv");

            // Ensure that the files exists, and create if necessary 
            if (!dbFile.Exists())
            {
                dbFile.Create(1000000); // Create 1mb file

                // Write CSV header
                string hdrText = "id,title,isComplete\r\n";
                this.hdrBytes = Encoding.ASCII.GetBytes(hdrText);
                dbFile.WriteRange(new MemoryStream(this.hdrBytes), 0);

                // Populate with a few entries
                this.RebuildList();
            }
        }

        public Boolean RebuildList()
        {
            List<string> newLines = new List<string>();
            newLines.Add("id,title,isComplete");
            newLines.Add("1,Make Tea,False");
            newLines.Add("2,Bake some bread,False");
            newLines.Add("3,Update CV,False");
            newLines.Add("4,Install visual studio,False");
            newLines.Add("5,Drink some water,False");
            newLines.Add("6,Get Some Milk,False");
            newLines.Add("7,Go for a Jog,False");
            newLines.Add("8,Feed the cat,False");
            newLines.Add("9,Cook some dinner,False");
            newLines.Add("10,Stretch,False");
            newLines.Add("11,Fix computer,False");
            newLines.Add("12,Purchase mouse,False");
            newLines.Add("13,Refactor Code,False");
            newLines.Add("14,Review C# style guide,False");
            newLines.Add("15,Remove template bloat,False");
            newLines.Add("16,Place js in bundle,False");
            newLines.Add("17,Test program,False");
            string linesString = string.Join("\r\n", newLines.ToArray());
            byte[] linesBytes = Encoding.ASCII.GetBytes(linesString);
            dbFile.UploadFromByteArray(linesBytes, 0, linesBytes.Length);
            return true;
        }

        public List<TaskEntry> GetRange(int offset, int count)
        {
            // Read entire CSV
            string contents = dbFile.DownloadTextAsync().Result;
            List<TaskEntry> tasks = new List<TaskEntry>();
            int endOffset = offset + count;

            IEnumerable<ICsvLine> lines = CsvReader.ReadFromText(contents, this.csvOptions);
            // Check we have enough entries
            if (lines.Count() < endOffset) endOffset = lines.Count();

            for (int i = offset; i < endOffset; i++)
            {
                ICsvLine line = lines.ElementAt(i);
                try
                {
                    tasks.Add(new TaskEntry(Int32.Parse(line["id"]), line["title"], System.Convert.ToBoolean(line["isComplete"])));
                } catch
                {
                    // Unable to parse entry or final blank line in CSV
                }
            }
            return tasks;
        }

        public Boolean Remove(int id)
        {
            // Read entire CSV
            string contents = dbFile.DownloadTextAsync().Result;
            List<string> newLines = new List<string>();
            newLines.Add("id,title,isComplete");

            // Find the line to delete
            IEnumerable<ICsvLine> lines = CsvReader.ReadFromText(contents, this.csvOptions);
            for (int i = 0; i < lines.Count(); i++)
            {
                ICsvLine line = lines.ElementAt(i);

                try
                {
                    if (Int32.Parse(line["id"]) == id)
                    {
                        // Log ?
                    } else
                    {
                        newLines.Add(line.Raw);
                    }
                }
                catch
                {
                    // Ignore lines which are not parsable 
                }
            }

            string linesString = string.Join("\r\n", newLines.ToArray());
            byte[] linesBytes = Encoding.ASCII.GetBytes(linesString);

            // overwrite db
            dbFile.UploadFromByteArray(linesBytes, 0, linesBytes.Length);

            return true;
        }

        public Boolean ToggleComplete(int id)
        {
            // Read entire CSV
            string contents = dbFile.DownloadTextAsync().Result;
            List<string> newLines = new List<string>();
            newLines.Add("id,title,isComplete");

            // Find the line to toggle
            IEnumerable<ICsvLine> lines = CsvReader.ReadFromText(contents, this.csvOptions);
            for (int i = 0; i < lines.Count(); i++)
            {
                ICsvLine line = lines.ElementAt(i);

                try
                {
                    if (Int32.Parse(line["id"]) == id)
                    {
                        string lineStr = line.Raw;
                        string[] lineCols = lineStr.Split(',');
                        if (lineCols[2] == "True")
                        {
                            lineCols[2] = "False";
                        } else
                        {
                            lineCols[2] = "True";
                        }
                        newLines.Add(string.Join(",", lineCols.ToArray()));
                    }
                    else
                    {
                        newLines.Add(line.Raw);
                    }
                }
                catch
                {
                    // Ignore lines which are not parsable 
                }
            }

            string linesString = string.Join("\r\n", newLines.ToArray());
            byte[] linesBytes = Encoding.ASCII.GetBytes(linesString);

            // overwrite db
            dbFile.UploadFromByteArray(linesBytes, 0, linesBytes.Length);

            return true;
        }

        public bool Insert(string title)
        {
            // Read entire CSV
            string contents = dbFile.DownloadTextAsync().Result;
            List<string> newLines = new List<string>();
            newLines.Add("id,title,isComplete");

            // Cycle through CSV lines and populate a new list
            IEnumerable<ICsvLine> lines = CsvReader.ReadFromText(contents, this.csvOptions);
            for (int i = 0; i < lines.Count(); i++)
            {
                ICsvLine line = lines.ElementAt(i);
                try
                {
                    // Check id is parsable, otherwise remove the entry
                    Int32.Parse(line["id"]);
                    newLines.Add(line.Raw);
                }
                catch
                {
                    // Ignore lines which are not parsable 
                }
            }

            // Insert the new entry into the new list
            Random random = new Random();
            int id = random.Next();
            string[] lineCols = { id.ToString(),title,"false"};
            newLines.Add(string.Join(",", lineCols.ToArray()));

            string linesString = string.Join("\r\n", newLines.ToArray());
            byte[] linesBytes = Encoding.ASCII.GetBytes(linesString);

            // overwrite db
            dbFile.UploadFromByteArray(linesBytes, 0, linesBytes.Length);

            return true;
        }
    }
}