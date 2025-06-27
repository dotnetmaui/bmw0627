using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Dot4Demo
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new();
        private const string TestUrl = "https://jojangwon.app.n8n.cloud/webhook-test/getimage";
        private const string ProdUrl = "https://jojangwon.app.n8n.cloud/webhook/getimage";
        private const string ApiKey = "dot4";

        public MainPage()
        {
            InitializeComponent();
            ModeSwitch.IsToggled = true; // 기본은 테스트 모드
        }

        private async void OnGenerateClicked(object sender, EventArgs e)
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            ResultImage.Source = null;
            GenerateButton.IsEnabled = false;

            try
            {
                var url = ModeSwitch.IsToggled ? TestUrl : ProdUrl;
                var imgPath = ImagePathEntry.Text?.Trim();
                if (string.IsNullOrEmpty(imgPath))
                {
                    await DisplayAlert("오류", "이미지 경로를 입력하세요.", "확인");
                    return;
                }

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("apikey", ApiKey);
                request.Content = new StringContent($"{{\"imgPath\":\"{imgPath}\"}}", System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("오류", $"이미지 생성 실패: {response.StatusCode}", "확인");
                    return;
                }

                var stream = await response.Content.ReadAsStreamAsync();
                ResultImage.Source = ImageSource.FromStream(() => stream);
            }
            catch (Exception ex)
            {
                await DisplayAlert("오류", $"예외 발생: {ex.Message}", "확인");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                GenerateButton.IsEnabled = true;
            }
        }
    }
}
