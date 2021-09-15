using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entry = ProcedUnity.ExecutionRegistry.Entry;


namespace ProcedUnity
{
    /// <summary>
    /// Base abstract class for interpreters of execution registries.
    /// Expected to analyze entry data to create text files, report to external APIs, create register report interfaces, etc.
    /// </summary>
    public abstract class ExecutionRegistryInterpreter
    {
        protected delegate TResult SolverFunc<in T1, in T2, out TResult>(T1 obj1, T2 obj2);

        private interface ISolver
        {
            bool SolveEntry(Entry entry);
        }

        private sealed class Solver<T> : ISolver where T : IExecution
        {
            private readonly SolverFunc<T, Entry, bool> func;

            public Solver(SolverFunc<T, Entry, bool> func)
            {
                Debug.Assert(func != null);
                this.func = func;
            }

            public bool SolveEntry(Entry entry) 
            {
                if(entry.source is T execution)
                {
                    return func.Invoke(execution, entry);
                }

                Debug.Assert(false, "INVALID_TYPE: " + entry.source.GetType());
                return false;
            }
        }

        private readonly Dictionary<string, ISolver> solvers = new Dictionary<string, ISolver>();


        protected void AddSolver<T>(string key, SolverFunc<T, Entry, bool> solverFunc) where T : IExecution
        {
            solvers.Add(key, new Solver<T>(solverFunc));
        }

        protected bool TrySolve(Entry entry)
        {
            return TrySolve(entry, out _);
        }

        protected bool TrySolve(Entry entry, out bool entryKeyNotFoundFlag)
        {
            if(solvers.TryGetValue(entry.key, out var solver))
            {
                entryKeyNotFoundFlag = true;
                return solver.SolveEntry(entry);
            }

            entryKeyNotFoundFlag = false;
            return false;
        }
    }
}