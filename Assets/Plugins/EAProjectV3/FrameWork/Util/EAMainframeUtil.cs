using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EAMainframeUtil
{
    // Creates the managers subordinate to MainFrame in the tree.
    static public EAMainFrame CreateMainFrameTree()
    {
        EAMainFrame mainFrame = EAMainFrame.instance;

        EAFrameUtil.Call<GameResourceManager>(GameResourceManager.instance);
        EAFrameUtil.Call<UIManager>(UIManager.instance);
        EAFrameUtil.Call<EASceneLoadingManager>(EASceneLoadingManager.instance);
        EAFrameUtil.Call<EACObjManager>(EACObjManager.instance);
        EAFrameUtil.Call<EA_ItemManager>(EA_ItemManager.instance);

        return mainFrame;
    }
    
}
