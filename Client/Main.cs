using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp;

namespace Client
{
    public partial class Main : Form
    {
        
        public bool handlerOpened = false;

        public Main()
        {
            InitializeComponent();
            InitializeUsersDataGridView();
            RefreshUserData();
            IsLoggedIn();
        }

        RestClient restClient = new RestClient("http://localhost/beadando/u4ufuk/users.php");
        RestClient loginClient = new RestClient("http://localhost/beadando/u4ufuk/login.php");
        RestClient gameClient = new RestClient("http://localhost/beadando/u4ufuk/game.php");

        bool IsLoggedIn()
        {
            if (CurrentUser.username == null)
            {
                ManageButtons(false);
                new UserHandler(loginClient, this, RequestType.LOGIN).Show();
                return false;
            }
            return true;
        }

        void InitializeUsersDataGridView()
        {
            usersData.Columns.Add("rank", "Rank");
            
            usersData.Columns.Add("username", "Username");
            usersData.Columns.Add("how_many_wins", "Wins");
            usersData.Columns.Add("id", "ID");
            usersData.Columns.Add("password", "Password");

            
        }

        public void RefreshUserData()
        {
            RestRequest request = new RestRequest();
            try
            {
                RestResponse response = restClient.Get(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(response.StatusDescription);
                }
                else
                {                    
                    Response res = restClient.Deserialize<Response>(response).Data;
                    List<User> users = res.Users;
                    users = users.OrderByDescending(x => x.how_many_wins).ToList();
                    usersData.Rows.Clear();
                    int rank = 1;
                    foreach (var user in users)
                    {
                        
                        usersData.Rows.Add(rank, user.username, user.how_many_wins, user.ID, user.password);
                        rank++;
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        public void RefreshLabels(string username)
        {
            RestRequest request = new RestRequest();
            try
            {
                RestResponse response = restClient.Get(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(response.StatusDescription);
                }
                else
                {
                    Response res = restClient.Deserialize<Response>(response).Data;
                    List<User> users = res.Users;
                    List<User> correctUser = users.Where(x => x.username == username).ToList();
                    User user = correctUser.First();
                    CurrentUser.Id = user.ID;
                    CurrentUser.balance = user.balance;
                    CurrentUser.how_many_wins = user.how_many_wins;
                    CurrentUser.user_type_id = user.user_type_id;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        public void ShowLoggedInUserInfo()
        {
            currentUserLabel.Text = CurrentUser.username;
            winsLabel.Text = CurrentUser.how_many_wins.ToString();
            balanceLabel.Text = CurrentUser.balance.ToString();
        }

        

        public void logoutButton_Click(object sender, EventArgs e)
        {
            CurrentUser.username = null;
            
            currentUserLabel.Text = "-";
            winsLabel.Text = "-";
            balanceLabel.Text = "-";

            ManageButtons(false);
            new UserHandler(loginClient, this, RequestType.LOGIN).Show();
        }



        public void loginButton_Click(object sender, EventArgs e)
        {
            if (!handlerOpened)
            {
                
                new UserHandler(loginClient, this, RequestType.LOGIN).Show();
            }
            else
            {
                MessageBox.Show("A handler window is already opened!");
            }
            
            
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void uploadMoney_Click(object sender, EventArgs e)
        {
            if (IsLoggedIn())
            {
                new UserHandler(restClient, this, RequestType.PUT).Show();
            }
        }

        private void registrationButton_Click(object sender, EventArgs e)
        {
            if (!handlerOpened)
            {
                new UserHandler(restClient, this, RequestType.POST).Show();
            }
            
        }

        public void ManageButtons(bool enable)
        {
            gameButton.Enabled = enable;
            logoutButton.Enabled = enable;
            loginButton.Enabled = !enable;
            uploadMoney.Enabled = enable;
            registrationButton.Enabled = !enable;
        }
    }
}
