using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

partial class EAActor
{
    protected EAFSMMaker fsmMaker = new EAFSMMaker();
    protected virtual void InitializeFSM() => fsmMaker.Initialize(Id);
    protected virtual void UpdateFSM() => fsmMaker.OnUpdate();
    protected virtual void ReleaseFSM() => fsmMaker.Close(); 
    
}