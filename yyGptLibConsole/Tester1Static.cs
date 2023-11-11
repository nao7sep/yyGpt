﻿using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using yyGptLib;
using yyLib;

namespace yyGptLibConsole
{
    public static class Tester1
    {
        public static void Test ()
        {
            yyUserSecretsModel xUserSecrets = new yyUserSecretsLoader ().Load ();
            var xConnectionInfo = new yyGptChatConnectionInfoModel { ApiKey = xUserSecrets.OpenAi!.ApiKey! };

            var xRequest = new yyGptChatRequestModel
            {
                Model = "gpt-4",
                N = 3
            };
            
            xRequest.AddMessage (yyGptChatMessageRole.System, "You are a helpful assistant.");

            yyGptChatResponseParser xParser = new ();

            using (yyGptChatClient xClient = new (xConnectionInfo))
            {
                try
                {
                    xRequest.AddMessage (yyGptChatMessageRole.User, "A riddle that has multiple answers, please.");
                    var xSendingTask1 = xClient.SendAsync (xRequest);

                    // Various tasks...

                    xSendingTask1.Wait ();

                    var xReadingTask1 = xClient.ReadToEndAsync ();

                    // Various tasks...

                    xReadingTask1.Wait ();

                    string? xJson = xReadingTask1.Result;
                    var xResponse1 = xParser.Parse (xJson);

                    if (xSendingTask1.Result.HttpResponseMessage.IsSuccessStatusCode)
                    {
                        foreach (var xChoice in xResponse1.Choices!)
                            Console.WriteLine ($"Message: {xChoice.Message!.Content}");

                        xRequest.AddMessage (yyGptChatMessageRole.Assistant, xResponse1.Choices [Random.Shared.Next (0, xResponse1.Choices.Count)].Message!.Content!);
                    }

                    else
                    {
                        // I havent found any official documentation on the error model.
                        // A roundtrip is tried to make sure all the properties are covered.

                        // I might also cover the moderation model, but I couldnt be bad enough to get moderated.
                        // https://platform.openai.com/docs/api-reference/moderations/object
                        // Maybe, the API's priority is to refuse to respond and apologize.

                        Console.WriteLine (xJson);

                        Console.WriteLine (JsonSerializer.Serialize (xResponse1, new JsonSerializerOptions
                        {
                            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                            WriteIndented = true
                        }));
                    }
                }

                catch (Exception xException)
                {
                    Console.WriteLine (xException);
                }

                try
                {
                    xRequest.Stream = true;
                    xRequest.AddMessage (yyGptChatMessageRole.User, "What is the answer?");
                    var xSendingTask2 = xClient.SendAsync (xRequest);

                    // Various tasks...

                    xSendingTask2.Wait ();

                    if (xSendingTask2.Result.HttpResponseMessage.IsSuccessStatusCode)
                    {
                        yyAutoExpandingList <StringBuilder> xBuilders = new ();

                        while (true)
                        {
#pragma warning disable CA2012 // Use ValueTasks correctly
                            var xReadingTask2 = xClient.ReadLineAsync ();
#pragma warning restore CA2012

                            // Various tasks?

                            // ChatGPT says:

                                // Task vs. ValueTask:
                                // - Task: A reference type for asynchronous operations. Common and heap-allocated.
                                //   Use for true async operations or infrequent method calls.
                                // - ValueTask: A value type for optimizing certain async scenarios, especially
                                //   when operations often complete synchronously or very quickly. Helps reduce
                                //   heap allocations compared to Task.

                                // Handling ValueTask without async/await:

                                // 1. Using ValueTask.Result directly (risky if ValueTask hasn't completed):
                                //    - Safe only if you are certain the operation has completed.
                                //    - Risk: Accessing Result on an incomplete ValueTask can block or throw exceptions.

                                //    ValueTask<int> myValueTask = SomeMethod();
                                //    int result = myValueTask.Result; // Only if sure it's completed

                                // 2. Using GetAwaiter().GetResult() (can block or cause deadlocks):
                                //    - Blocks the calling thread until ValueTask completes.
                                //    - Risk: Can cause deadlocks in contexts with synchronization contexts (e.g., UI apps).

                                //    int result = myValueTask.GetAwaiter().GetResult(); // Blocks the calling thread

                                // 3. Converting to Task using AsTask() (negates ValueTask performance benefits):
                                //    - Useful for working with APIs that require a Task.
                                //    - Risk: Frequent conversion to Task can eliminate the performance advantages of ValueTask.

                                //    Task<int> task = myValueTask.AsTask();
                                //    task.ContinueWith(t => { /* ... */ });

                                // Note: ValueTask is designed for use with async and await. Not using these constructs
                                // requires careful handling to avoid issues like deadlocks or blocking threads. If these
                                // risks are a concern, consider using Task with async/await for a safer approach.

                            xReadingTask2.AsTask ().Wait ();

                            string? xLine = xReadingTask2.Result;

                            if (string.IsNullOrWhiteSpace (xLine))
                                continue; // Continues for "data: [DONE]".

                            var xResponse2 = xParser.ParseChunk (xLine);

                            if (xResponse2 == yyGptChatResponseModel.Empty)
                                break; // "data: [DONE]" is detected.

                            string? xContent = xResponse2.Choices! [0].Delta!.Content;

                            // ChatGPT may return a chunk only with line breaks.

                            // if (string.IsNullOrWhiteSpace (xContent))
                                // continue;

                            int xIndex = xResponse2.Choices [0].Index!.Value;

                            xBuilders [xIndex].Append (xContent);
                            Console.WriteLine (FormattableString.Invariant ($"Read Chunk: [{xIndex}] {xContent}"));
                        }

                        foreach (StringBuilder xBuilder in xBuilders)
                            Console.WriteLine ($"Message: {xBuilder}");
                    }

                    else
                    {
                        var xReadingTask2 = xClient.ReadToEndAsync ();

                        string? xJson = xReadingTask2.Result;
                        var xResponse2 = xParser.Parse (xJson);

                        // Again, to make sure all the properties are covered.

                        Console.WriteLine (xJson);

                        Console.WriteLine (JsonSerializer.Serialize (xResponse2, new JsonSerializerOptions
                        {
                            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                            WriteIndented = true
                        }));
                    }
                }

                catch (Exception xException)
                {
                    Console.WriteLine (xException);
                }
            }
        }
    }
}
