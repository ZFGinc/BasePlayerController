using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ZFGinc.Utils
{
    public class ImageLoader
    {
        // Метод для загрузки изображения и создания спрайта
        public IEnumerator LoadImageAsSprite(string url, System.Action<Sprite> onLoaded)
        {
            // Создаем запрос для загрузки данных
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Отправляем запрос и ждем завершения
                yield return request.SendWebRequest();

                // Проверяем, были ли ошибки
                if (request.result != UnityWebRequest.Result.Success)
                {
                    UnityEngine.Debug.LogError($"Failed to load image from {url}: {request.error}");
                    onLoaded?.Invoke(null); // Передаем null, если загрузка не удалась
                    yield break;
                }

                // Получаем байты из ответа
                byte[] imageData = request.downloadHandler.data;

                // Преобразуем байты в текстуру
                Texture2D texture = new Texture2D(2, 2); // Создаем пустую текстуру
                if (texture.LoadImage(imageData)) // Загружаем данные в текстуру
                {
                    // Преобразуем текстуру в спрайт
                    Sprite sprite = CreateSpriteFromTexture(texture);

                    // Вызываем callback с результатом
                    onLoaded?.Invoke(sprite);
                }
                else
                {
                    UnityEngine.Debug.LogError("Failed to load image data into Texture2D.");
                    onLoaded?.Invoke(null);
                }
            }
        }

        // Метод для создания спрайта из текстуры
        private Sprite CreateSpriteFromTexture(Texture2D texture)
        {
            if (texture == null)
            {
                UnityEngine.Debug.LogError("Texture is null. Cannot create sprite.");
                return null;
            }

            // Создаем спрайт с использованием всей текстуры
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f); // Центральный пивот
            float pixelsPerUnit = 100f; // Стандартное значение для спрайтов

            return Sprite.Create(texture, rect, pivot, pixelsPerUnit);
        }
    }
}