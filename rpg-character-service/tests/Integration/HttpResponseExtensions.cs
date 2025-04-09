namespace RPGCharacterService.IntegrationTests {
    public static class HttpResponseExtensions {
        public static async Task EnsureSuccessStatusCodeWithLogging(this HttpResponseMessage response) {
            if (!response.IsSuccessStatusCode) {
                var content = await response.Content.ReadAsStringAsync();
                var headers = string.Join(", ", response.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"));

                Console.WriteLine($"Request failed with status: {response.StatusCode}");
                Console.WriteLine($"Response Headers: {headers}");
                Console.WriteLine($"Response Content: {content}");

                // Try to parse as JSON for better readability
                try {
                    var jsonContent = System.Text.Json.JsonSerializer.Deserialize<dynamic>(content);
                    Console.WriteLine($"Parsed JSON Content: {System.Text.Json.JsonSerializer.Serialize(jsonContent, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");
                } catch {
                    // If it's not JSON, we already logged the raw content
                }

                throw new HttpRequestException($"Request failed with status {response.StatusCode}. See console for details.");
            }
        }
    }
}
