using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;

using Aliyun.Acs.Dysmsapi.Model.V20170525;


namespace ConsoleApplication1
{
    class Program
    {
        static String product = "Dysmsapi";//����API��Ʒ����
        static String domain = "dysmsapi.aliyuncs.com";//����API��Ʒ����
        static String accessId = "";
        static String accessSecret = "";
        static String regionIdForPop = "cn-hangzhou";
        static void Main(string[] args)
        {


            IClientProfile profile = DefaultProfile.GetProfile(regionIdForPop, accessId, accessSecret);
            DefaultProfile.AddEndpoint(regionIdForPop, regionIdForPop, product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try
            {
                //request.SignName = "����Ԥ������";//"�������̨�����õĶ���ǩ����״̬��������֤ͨ����"
                //request.TemplateCode = "SMS_71130001";//�������̨�����õ����ͨ���Ķ���ģ���ģ��CODE��״̬��������֤ͨ����"
                //request.RecNum = "13567939485";//"���պ��룬���������Զ��ŷָ�"
                //request.ParamString = "{\"name\":\"123\"}";//����ģ���еı�����������Ҫת��Ϊ�ַ����������û�ÿ���������ȱ���С��15���ַ���"
                //SingleSendSmsResponse httpResponse = client.GetAcsResponse(request);
                request.PhoneNumbers = "1350000000";
                request.SignName = "xxxxxx";
                request.TemplateCode = "SMS_xxxxxxx";
                request.TemplateParam = "{\"code\":\"123\"}";
                request.OutId = "xxxxxxxx";
                //����ʧ���������ClientException�쳣
                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                System.Console.WriteLine(sendSmsResponse.Message);

            }
            catch (ServerException e)
            {
                System.Console.WriteLine("ServerException");
            }
            catch (ClientException e)
            {
                System.Console.WriteLine("ClientException");
            }
        }
    }
}
