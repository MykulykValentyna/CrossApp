namespace Cli;

using System;
using System.IO;
using System.Linq;
using Core.Abstractions;
using Core.Storage;

public static class StoreFactory
{
    public static IBookStore Create(string[] args)
    {
        bool useFile = args.Contains("--file");
        bool useCache = args.Contains("--cache"); 
        
        string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");
        
        IBookStore store = useFile 
            ? new FileBookStore(dataPath) 
            : new InMemoryBookStore();
            
        if (useCache)
        {
            store = new CachingBookStore(store);
        }
        
        return store;
    }
}