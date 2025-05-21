using System;
using System.IO;
using System.Web;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using VI.Base;
using VI.DB;
using VI.DB.Entities;
using QBM.CompositionApi.ApiManager;
using QBM.CompositionApi.Definition;
using QBM.CompositionApi.Crud;
using QER.CompositionApi.Portal;
using VI.DB.DataAccess;
using VI.DB.Sync;
using System.Runtime.ConstrainedExecution;

namespace QBM.CompositionApi
{
    public class PredefinedSQL : IApiProviderFor<QER.CompositionApi.Portal.PortalApiProject>, IApiProvider // This is a comment to test BRANCH
    {
        public void Build(IApiBuilder builder)
        {
            builder.AddMethod(Method.Define("PredefinedSQL/predefinedsql")
                  .Handle<PostedSQL, List<List<ColumnData>>>("POST", async (posted, qr, ct) =>
                  {
                      //var strUID_Person = qr.Session.User().Uid;                // Retrieve the UID of the currently logged-in user from the session 

                      var results = new List<List<ColumnData>>();               // Initialize a list to hold the results, each result is a list of ColumnData objects (rows)

                      var runner = qr.Session.Resolve<IStatementRunner>();      // Resolve an instance of IStatementRunner from the session to execute SQL statements

                      using (var reader = runner.SqlExecute(posted.IdentQBMLimitedSQL, new[]        // Execute a predefined SQL statement with parameters
                      {
                          //QueryParameter.Create("uidperson", strUID_Person),            // Pass parameters to the SQL query
                          QueryParameter.Create("Description", posted.Description),
                          QueryParameter.Create("Mail", posted.Mail),
                          QueryParameter.Create("MailNickName", posted.MailNickName)
                      }))
                      {
                          while (reader.Read())                                     // Read each row returned by the SQL query
                          {
                              var row = new List<ColumnData>();                     // Initialize a list to hold the columns for the current row
                              for (int i = 0; i < reader.FieldCount; i++)           // Loop through each field (column) in the current row
                              {
                                  row.Add(new ColumnData                            // Add the column name and value to the row
                                  {
                                      Column = reader.GetName(i),
                                      Value = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString()
                                  });
                              }
                              results.Add(row);                                     // Add the row to the results list
                          }
                      }
                      return results;               // Return the results as a list of lists of ColumnData objects
                  }));
        }

        public class ColumnData             // The ColumnData class represents a single column and its value in a database row
        {
            public string Column { get; set; }
            public string Value { get; set; }
        }

        public class PostedSQL              // The PostedSQL class represents the data structure of the POST request body
        { 
            public string IdentQBMLimitedSQL { get; set; }          // The identifier of the predefined SQL statement to execute
            public string Description { get; set; }                        // Additional parameters to pass to the SQL statement
            public string Mail { get; set; }
            public string MailNickName { get; set; }
        }
    }
}
