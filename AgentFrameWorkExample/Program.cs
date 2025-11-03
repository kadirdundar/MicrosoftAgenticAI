using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using System.Text.Json;
using Microsoft.Agents.AI.Workflows;


// See https://aka.ms/new-console-template for more information
using System;


using OllamaApiClient chatClient = new(new Uri("http://localhost:11434"), "qwen3:1.7b");




// AIAgent agent = new ChatClientAgent(
//     chatClient,
//     instructions: "You are good at telling jokes.",
//     name: "Joker");
// // Invoke the agent and output the text result.
// Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));


AIAgent writer = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "Writer",
        Instructions = "Write joke that are engaging and creative."
    });

AIAgent translator = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "Translator",
        Instructions = "Translate the text to Turkish."
    });

AIAgent editor = new ChatClientAgent(
chatClient,
new ChatClientAgentOptions
{
    Name = "Editor",
    Instructions = "Make the story more engaging, fix grammar, and enhance the plot."
});


//  Workflow workflow =
//      AgentWorkflowBuilder
//          .BuildSequential(writer, editor);

// AIAgent workflowAgent = await workflow.AsAgent(); 

// AgentRunResponse workflowResponse =
//     await workflowAgent.RunAsync("Write a short story about a haunted house.");

// Console.WriteLine(workflowResponse.Text);

 var workflow = new WorkflowBuilder(writer)
            .AddEdge(writer, translator)
            .AddEdge(translator, editor)
            .Build();

        // Execute the workflow
await using StreamingRun run = await InProcessExecution.StreamAsync(workflow, new ChatMessage(ChatRole.User, "Hello World!"));

await foreach (WorkflowEvent evt in run.WatchStreamAsync())
        {
            if (evt is AgentRunUpdateEvent executorComplete)
            {
                Console.WriteLine($"{executorComplete.Data}");
            }
        }

//  await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
//   await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
//        var lastMessage = new Dictionary<string, System.Text.StringBuilder>();
//        await foreach (WorkflowEvent evt in run.WatchStreamAsync())
//        {
//            if (evt is AgentRunUpdateEvent agentRunUpdate)
//            {
//                if (!lastMessage.TryGetValue(agentRunUpdate.ExecutorId, out var builder))
//                {
//                    builder = new System.Text.StringBuilder();
//                    lastMessage[agentRunUpdate.ExecutorId] = builder;
//                }
//                builder.Append(agentRunUpdate.Data);
//            }
//        }

  
