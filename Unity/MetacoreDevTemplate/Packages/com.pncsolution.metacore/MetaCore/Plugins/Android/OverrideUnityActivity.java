package kr.co.pncsolution.aidlmodule.unity;
import android.os.Bundle;
import android.util.Log;

import com.unity3d.player.UnityPlayerActivity;

public abstract class OverrideUnityActivity extends UnityPlayerActivity
{
    private final static String TAG = "OverrideUnityActivity";
    public static OverrideUnityActivity instance = null;

    abstract protected void showMainActivity(String setToColor);

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        instance = this;
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        instance = null;
    }
}
