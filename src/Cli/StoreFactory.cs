namespace Cli;

using System;
using System.IO;
using System.Linq;
using Core.Abstractions;
using Core;
using Core.Storage;

public static class StoreFactory
{
    public static IBookStore Create(string[] args)
    {
        bool useFile = args.Contains("--file");
        bool useCache = args.Contains("--cache"); 
        
        string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");
        
        IBookStore store;
        if (useFile)
        {
            var fileStore = new FileBookStore(dataPath);
            if (!File.Exists(dataPath))
            {
                foreach (var book in SampleData.Books)
                    fileStore.Add(book);
            }

            store = fileStore;
        }
        else
        {
            store = new InMemoryBookStore(SampleData.Books);
        }
            
        if (useCache)
        {
            store = new CachingBookStore(store);
        }
        
        return store;
    }
}