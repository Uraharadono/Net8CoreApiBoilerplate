using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Net7CoreApiBoilerplate.DbContext.Interceptors
{
    /* ----------------------------------------------------------------------------
     * Explanation as why I (personaly) found need for interceptor logic:
     * Interceptors can be used to well intercept any SQL code that is about to be executed on database.
     * 
     * If you take a look at Context of this project, you will see that instead of Identity on Blog.Id, I have used BlogSeq.
     * Sometimes we cannot set the sequence like we did above for the BlogSeq. Sequences are already feeling "hacky",
     * even though "Identity" on columns (mainly PK's) use them under the hood.
     * 
     * In my case, reason for this interceptor is that we have had an old system that is still being used while we reworked this one. 
     * Records for Logging table are being added constantly in both systems (new and old).
     * Old system uses some obscure logic to fetch next id for each insert of new Logging record, 
     * and in new system if want to insert new records, we have to reset Sequence before we insert our records. 
     * 
     * So our Intereceptor insertion logic goes like this:
     * 0. Is insret into Logging table command run (basically find string "INSERT INTO [Logging]")
     * 1. Reset "LoggingSeq" to: "Last Logging record ID + 1"
     * 2. Replace 1st value in "VALUES(p[0], ... " with string "NEXT VALUE FOR LoggingSeq" statement.
     * 3. Save changes to this command and let it continue execution.
     * 
     * 
     * Notes: 1.) pay attention, there are async and non async interceptors
     *        2.) comments below are not referencing "why?" part anymore. They are my personal notes that I have decided to keep here.     
     ---------------------------------------------------------------------------- */
    public class LoggingInterceptor : DbCommandInterceptor
    {
        #region Synchronous interceptor (if you are NOT using async method on dbcontext)
        // Not gonna lie, I read the official documentation: https://docs.microsoft.com/en-us/ef/core/logging-events-diagnostics/interceptors
        // Especially TaggedQueryCommandInterceptor , but it made 0 sense honestly
        // But implementation without pulling whole example code I got from: https://stackoverflow.com/a/58331926/4267429
        public override InterceptionResult<DbDataReader> ReaderExecuting(
          DbCommand command,
          CommandEventData eventData,
          InterceptionResult<DbDataReader> result)
        {
            if (command.CommandText.Contains("INSERT INTO [Logging]"))
            {
                var adjustedCommandText = new StringBuilder();

                string[] lines = command.CommandText.Split(
                                new string[] { Environment.NewLine },
                                StringSplitOptions.None
                                );

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("INSERT INTO [Logging]"))
                    {
                        // Note: THIS IS HACK, but so is everything in this file
                        // To reset sequence every time logging record is being inserted
                        adjustedCommandText.AppendLine("DECLARE @LoggingSeqSeqResetSQL nvarchar(255) = 'ALTER SEQUENCE LoggingSeq RESTART WITH ' + CAST((SELECT TOP 1([OID] + 1) FROM[Logging] ORDER BY[OID] DESC) AS NVARCHAR(20));");
                        adjustedCommandText.AppendLine("exec sp_executesql @LoggingSeqSeqResetSQL;");

                        // Then do the transformations
                        adjustedCommandText.AppendLine(lines[i]);
                        i++;

                        while (lines[i].Contains("),"))
                        {
                            adjustedCommandText.AppendLine(ReplaceCommandParam(lines[i], "(", ",", "NEXT VALUE FOR LoggingSeq"));
                            i++;
                        }

                        // One last line with ");", but we have to check that it is not single record insert
                        if (lines[i].Contains(");"))
                        {
                            adjustedCommandText.AppendLine(ReplaceCommandParam(lines[i], "(", ",", "NEXT VALUE FOR LoggingSeq"));
                            // i++;
                        }
                    }
                    else
                    {
                        adjustedCommandText.AppendLine(lines[i]);
                    }
                }
                command.CommandText = adjustedCommandText.ToString();
            }

            return result;
        }
        #endregion

        private static string ReplaceCommandParam(string line, string firstExpression, string secondExpression, string replacementText)
        {
            int start = line.IndexOf(firstExpression);
            int end = line.IndexOf(secondExpression, start);
            string textResult = line.Substring(start + 1, end - start - 1);
            // return line.Replace(textResult, "NEXT VALUE FOR LoggingSeq");
            return line.Replace(textResult, replacementText);
        }

        #region Asynchronous interceptor (if you are using async method on dbcontext)
        // As per my question on the GitHub: https://github.com/dotnet/efcore/issues/29560
        // For async stuff, we need to do async interceptor
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            if (command.CommandText.Contains("INSERT INTO [Logging]"))
            {
                var adjustedCommandText = new StringBuilder();

                string[] lines = command.CommandText.Split(
                                new string[] { Environment.NewLine },
                                StringSplitOptions.None
                                );

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("INSERT INTO [Logging]"))
                    {
                        // Note: THIS IS HACK, but so is everything in this file
                        // To reset sequence every time logging record is being inserted
                        adjustedCommandText.AppendLine("DECLARE @LoggingSeqSeqResetSQL nvarchar(255) = 'ALTER SEQUENCE LoggingSeq RESTART WITH ' + CAST((SELECT TOP 1([Id] + 1) FROM[Logging] ORDER BY[Id] DESC) AS NVARCHAR(20));");
                        adjustedCommandText.AppendLine("exec sp_executesql @LoggingSeqSeqResetSQL;");

                        // Then do the transformations
                        adjustedCommandText.AppendLine(lines[i]);
                        i++;

                        while (lines[i].Contains("),"))
                        {
                            adjustedCommandText.AppendLine(ReplaceCommandParam(lines[i], "(", ",", "NEXT VALUE FOR LoggingSeq"));
                            i++;
                        }

                        // One last line with ");", but we have to check that it is not single record insert
                        if (lines[i].Contains(");"))
                        {
                            adjustedCommandText.AppendLine(ReplaceCommandParam(lines[i], "(", ",", "NEXT VALUE FOR LoggingSeq"));
                            // i++;
                        }
                    }
                    else
                    {
                        adjustedCommandText.AppendLine(lines[i]);
                    }
                }
                command.CommandText = adjustedCommandText.ToString();
            }

            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }
        #endregion
    }
}
