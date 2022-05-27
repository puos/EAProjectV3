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
        EAFrameUtil.Call<EAUIManager>(EAUIManager.instance);
        EAFrameUtil.Call<EASceneLoadingManager>(EASceneLoadingManager.instance);
        EAFrameUtil.Call<EASoundManager>(EASoundManager.instance);
        EAFrameUtil.Call<OptionManager>(OptionManager.instance);
        EAFrameUtil.Call<EACObjManager>(EACObjManager.instance);
        EAFrameUtil.Call<EA_ItemManager>(EA_ItemManager.instance);
        EAFrameUtil.Call<EASfxManager>(EASfxManager.instance);
        EAFrameUtil.Call<EAEventManager>(EAEventManager.instance);

        return mainFrame;
    }
    
}
