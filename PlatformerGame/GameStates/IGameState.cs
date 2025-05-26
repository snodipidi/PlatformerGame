using System.Drawing;
using System.Windows.Forms;
using System;

/// <summary>
/// Интерфейс для всех состояний игры (State pattern)
/// </summary>
/// <remarks>
/// Определяет базовый контракт для различных состояний игры,
/// таких как меню, игровой процесс, пауза, завершение игры и т.д.
/// </remarks>
public interface IGameState
{
    /// <summary>
    /// Обновляет логику состояния игры
    /// </summary>
    /// <remarks>
    /// Вызывается каждый кадр перед отрисовкой.
    /// </remarks>
    void Update();

    /// <summary>
    /// Отрисовывает состояние игры
    /// </summary>
    /// <param name="g">Объект Graphics для отрисовки</param>
    /// <remarks>
    /// Вызывается каждый кадр после Update().
    /// Все графические операции выполняются здесь.
    /// </remarks>
    void Draw(Graphics g);

    /// <summary>
    /// Обрабатывает ввод с клавиатуры
    /// </summary>
    /// <param name="e">Аргументы события клавиши</param>
    /// <remarks>
    /// Вызывается при нажатии клавиш.
    /// </remarks>
    void HandleInput(KeyEventArgs e);

    /// <summary>
    /// Обрабатывает клики мыши
    /// </summary>
    /// <param name="e">Аргументы события мыши</param>
    /// <remarks>
    /// Вызывается при кликах мышью.
    /// Может обрабатывать как клики, так и движение мыши.
    /// </remarks>
    void HandleMouseClick(MouseEventArgs e);

    /// <summary>
    /// Вызывается при входе в это состояние
    /// </summary>
    /// <remarks>
    /// Инициализирует состояние, загружает ресурсы,
    /// сбрасывает временные параметры.
    /// </remarks>
    void OnEnter();

    /// <summary>
    /// Вызывается при выходе из этого состояния
    /// </summary>
    /// <remarks>
    /// Освобождает ресурсы, сохраняет данные,
    /// выполняет финализацию состояния.
    /// </remarks>
    void OnExit();

    /// <summary>
    /// Обрабатывает изменение размера окна
    /// </summary>
    /// <param name="e">Аргументы события</param>
    /// <remarks>
    /// Вызывается при изменении размеров игрового окна.
    /// Должен корректировать позиции UI элементов.
    /// </remarks>
    void OnResize(EventArgs e);
}