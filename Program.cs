#region License

/*
 * Eleia
 *
 * Copyright (C) Marcin Badurowicz <m at badurowicz dot net> 2019-2022
 *
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files
 * (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge,
 * publish, distribute, sublicense, and/or sell copies of the Software,
 * and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
 * BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
 * ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#endregion License

using Eleia;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

Eleia.ML.CodeDetector detector = new Eleia.ML.CodeDetector();

// listens on /detectunformattedcode for a POST request with a body and performs
// a classification on every line of a body returning a classification result as
// a JSON object with prediction (bool) probability (float, optional) and line
// which caused prediction to true (string, optional)
app.MapPost("/detectunformattedcode", async (HttpRequest request) =>
{
    using (var sr = new StreamReader(request.Body))
    {
        var markdown = await sr.ReadToEndAsync();
        var paras = TextCleaner.PrepareBody(markdown);

        foreach (var item in paras)
        {
            var predictionResult = detector.Predict(item);

            if (predictionResult.Prediction)
            {
                app.Logger.LogInformation($"Found unformatted code: {item} (prob: {predictionResult.Probability})");
                return Results.Ok(new { prediction = true, probability = predictionResult.Probability, item = item });
            }
        }
    }

    return Results.Ok(new { prediction = false });
});

app.Run();
