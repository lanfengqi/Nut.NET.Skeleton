using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Web.Hosting;
using System.IO;
using System.Text;

namespace Foundatio.Skeleton.Repositories.Configuration {
    public class EFConfiguration {

        public void ConfigureDatabase() {

            //pass some table names to ensure that we have nut.net installed
            var tablesToValidate = new[] { "Token", "Organization" };

            var customCommands = new List<string>();
            //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
            customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/Install/MySql.Indexes.sql"), false));

            var initializer = new CreateTablesIfNotExist<EFDbContext>(tablesToValidate, customCommands.ToArray());
            Database.SetInitializer(initializer);
        }

        protected virtual string[] ParseCommands(string filePath, bool throwExceptionIfNonExists) {
            if (!File.Exists(filePath)) {
                if (throwExceptionIfNonExists)
                    throw new ArgumentException(string.Format("Specified file doesn't exist - {0}", filePath));
                else
                    return new string[0];
            }


            var statements = new List<string>();
            using (var stream = File.OpenRead(filePath))
            using (var reader = new StreamReader(stream)) {
                var statement = "";
                while ((statement = readNextStatementFromStream(reader)) != null) {
                    statements.Add(statement);
                }
            }

            return statements.ToArray();
        }

        protected virtual string readNextStatementFromStream(StreamReader reader) {
            var sb = new StringBuilder();

            string lineOfText;

            while (true) {
                lineOfText = reader.ReadLine();
                if (lineOfText == null) {
                    if (sb.Length > 0)
                        return sb.ToString();
                    else
                        return null;
                }

                //MySql doesn't support GO, so just use a commented out GO as the separator
                if (lineOfText.TrimEnd().ToUpper() == "-- GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
