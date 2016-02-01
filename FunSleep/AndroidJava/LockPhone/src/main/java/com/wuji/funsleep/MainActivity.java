package com.wuji.funsleep;

import com.tencent.mm.sdk.modelmsg.SendMessageToWX;
import com.tencent.mm.sdk.modelmsg.WXMediaMessage;
import com.tencent.mm.sdk.modelmsg.WXTextObject;
import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayerActivity;

import android.app.Activity;
import android.app.admin.DevicePolicyManager;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

public class MainActivity extends UnityPlayerActivity
{
	private static final int MY_REQUEST_CODE = 9999;
	private DevicePolicyManager policyManager;
    private ComponentName componentName;
    private IWXAPI api;

    public void initWXApi(String appId)
    {
    	api = WXAPIFactory.createWXAPI(this, appId, true);
    	api.registerApp(appId);
    }

    public void sendTextToWX(String text)
    {
    	WXTextObject textObj = new WXTextObject();
    	textObj.text = text;

    	WXMediaMessage msg = new WXMediaMessage();
    	msg.mediaObject = textObj;
    	msg.description = text;

    	SendMessageToWX.Req req = new SendMessageToWX.Req();
    	req.transaction = buildTransaction("Text");
    	req.message = msg;
    	req.scene = SendMessageToWX.Req.WXSceneTimeline;
    	api.sendReq(req);
    }

    private String buildTransaction(String transcationType)
    {
    	return String.format("%s_%l", transcationType, System.currentTimeMillis());
	}

	/**
     * 锁屏
     */
	public void doLock()
    {
		Log.d("Unity", "start doLock");

    	//获取设备管理服务
    	policyManager = (DevicePolicyManager) getSystemService(Context.DEVICE_POLICY_SERVICE);
    	componentName = new ComponentName(this, AdminReceiver.class);

    	//判断是否有锁屏权限，若有则立即锁屏并结束自己，若没有则获取权限
    	if (policyManager.isAdminActive(componentName))
    	{
    		policyManager.lockNow();
    		finish();
    	} else
    	{
    		activeManage();
    	}
    }

  //获取权限
    private void activeManage()
    {
        // 启动设备管理(隐式Intent) - 在AndroidManifest.xml中设定相应过滤器
        Intent intent = new Intent(DevicePolicyManager.ACTION_ADD_DEVICE_ADMIN);

        //权限列表
        intent.putExtra(DevicePolicyManager.EXTRA_DEVICE_ADMIN, componentName);

        //描述(additional explanation)
        intent.putExtra(DevicePolicyManager.EXTRA_ADD_EXPLANATION, "激活后才能使用锁屏功能哦亲^^");

        startActivityForResult(intent, MY_REQUEST_CODE);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data)
    {
        //获取权限成功，立即锁屏并finish自己，否则继续获取权限
        if (requestCode == MY_REQUEST_CODE && resultCode == Activity.RESULT_OK)
        {
            policyManager.lockNow();
            finish();
        }
        else
        {
            activeManage();
        }
        super.onActivityResult(requestCode, resultCode, data);
    }
}
