using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZFreeGo.Monitor.AutoStudio.Secure;

namespace ZFreeGo.ChoicePhase.ControlPlatform.StartupUI
{
    /// <summary>
    /// StartupInterface.xaml 的交互逻辑
    /// </summary>
    public partial class StartupInterface : Window
    {

       /// <summary>
       /// 账户管理器
       /// </summary>
        private AccountManager accountManager;



        MainWindow main;
        /// <summary>
        /// 失败计数
        /// </summary>
        private int failueLoginCount = 0;

        private int errorCountLimit = 3;
        public StartupInterface()
        {
            InitializeComponent();
            main = new MainWindow();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
              
                accountManager = new AccountManager();
                var user =  accountManager.GetLastAccount();
                if( null != user)
                {
                    txtLoginUser.Text = user.UserName;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "载入账户信息");
            }

           
            
        }

        /// <summary>
        /// 登陆界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (failueLoginCount >= errorCountLimit)
            {
                var str = string.Format("密码错误大于{0}次，禁止登陆", errorCountLimit);
                MessageBox.Show(str, "登陆警告");
               
                return;
            }
           

            if (accountManager.LoginCheck(txtLoginUser.Text, passBox.SecurePassword))
            {
                var str = string.Format("登陆尝试{0}后登陆成功", failueLoginCount);
                
                
                

                main.Show();

                this.Close();
            }
            else
            {
                if (++failueLoginCount >= errorCountLimit)
                {
                    var str = string.Format("密码错误大于{0}次，禁止登陆", errorCountLimit);
                    MessageBox.Show(str, "登陆警告");
                   
                }
                else
                {
                    var str = string.Format("密码错误,还剩下{0}次机会", errorCountLimit - failueLoginCount);
                    MessageBox.Show(str, "登陆警告");
                   
                }
            }

            
        }


        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnlogCancel_Click(object sender, RoutedEventArgs e)
        {
            main.Close();
            this.Close();
        
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
          
            
        }


      
        
    }
}
