using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace util
{
  public class AndroidUtils {
    public static void showToast(string message)
    {
      AndroidJavaObject _currentActivity;
      AndroidJavaClass _unityPlayer;
      AndroidJavaObject _context;
      AndroidJavaObject _toast;

        _unityPlayer = 
        	new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        
        _currentActivity = _unityPlayer
        	.GetStatic<AndroidJavaObject>("currentActivity");
        	
        	
        _context = _currentActivity
        	.Call<AndroidJavaObject>("getApplicationContext");

        _currentActivity.Call
        (
	        "runOnUiThread", 
	        new AndroidJavaRunnable(() =>
	        {
	            AndroidJavaClass toastobject 
	            = new AndroidJavaClass("android.widget.Toast");
	            
	            AndroidJavaObject javaString 
	            = new AndroidJavaObject("java.lang.String", message);
	            
	            _toast = toastobject.CallStatic<AndroidJavaObject>
	            (
	            	"makeText", 
	            	_context, 
	            	javaString, 
	            	toastobject.GetStatic<int>("LENGTH_SHORT")
	            );
	            
	            _toast.Call("show");
	        })
	     );
    }

    public static void cancelToast()
    {
        // _currentActivity.Call("runOnUiThread", 
        // 	new AndroidJavaRunnable(() =>
	      //   {
	      //       if (_toast != null) _toast.Call("cancel");
	      //   }));
    }
  } //AndroidUtils
} //namespace util
