using System;
using System.Collections.Generic;
using _StoryGame.Core.Currency.Interfaces;
using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Game.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers.HUD.Components
{
    public struct InventoryCellData
    {
        public int Index;
        public VisualElement Cell;
        public CurrencyChangedData Item;
    }

    public sealed class InventoryHUDController : IDisposable
    {
        private const int CellCount = 5;

        private VisualElement _invContainer;
        private readonly Queue<InventoryCellData> _freeCells = new();
        private readonly Dictionary<int, InventoryCellData> _cellData = new();
        private readonly ICurrencyRegistry _currencyRegistry;

        public InventoryHUDController(ICurrencyRegistry currencyRegistry) => _currencyRegistry = currencyRegistry;

        public void Init(VisualElement mainContainer, VisualTreeAsset invCellTemplate)
        {
            _invContainer = mainContainer.Q<VisualElement>("inv-cont");

            for (var i = 0; i < CellCount; i++)
            {
                var cell = invCellTemplate.Instantiate();
                _invContainer.Add(cell);

                var cellData = new InventoryCellData
                {
                    Index = i,
                    Cell = cell,
                    Item = null
                };

                _cellData.Add(i, cellData);
                _freeCells.Enqueue(cellData);

                SetupCellVisuals(cell, cellData);
            }
        }

        private void SetupCellVisuals(VisualElement cell, InventoryCellData cellData)
        {
            var iconCont = cell.GetVisualElement<VisualElement>("icon", nameof(InventoryHUDController));
            var label = cell.GetVisualElement<Label>("quantity", nameof(InventoryHUDController));

            if (cellData.Item != null)
            {
                var item = cellData.Item;
                var icon = _currencyRegistry.GetIcon(item.Id);
                iconCont.style.backgroundImage = new StyleBackground(icon);
                label.text = item.NewAmount.ToString();
                iconCont.style.display = DisplayStyle.Flex;
                label.style.display = DisplayStyle.Flex;
            }
            else
            {
                iconCont.style.backgroundImage = null;
                label.text = "";
                iconCont.style.display = DisplayStyle.None;
                label.style.display = DisplayStyle.None;
            }
        }

        public void OnCurrencyChanged(CurrencyChangedData data)
        {
            // Попробуем найти существующую ячейку с этим предметом
            foreach (var cellData in _cellData.Values)
            {
                if (cellData.Item != null && cellData.Item.Id == data.Id)
                {
                    // Обновляем количество в существующей ячейке
                    var updatedItem = cellData.Item;
                    var updatedCellData = new InventoryCellData
                    {
                        Index = cellData.Index,
                        Cell = cellData.Cell,
                        Item = updatedItem
                    };
                    _cellData[cellData.Index] = updatedCellData;
                    SetupCellVisuals(cellData.Cell, updatedCellData);
                    return;
                }
            }

            // Если предмет новый и есть свободная ячейка
            if (_freeCells.Count > 0)
            {
                var freeCellData = _freeCells.Dequeue();
                freeCellData.Item = data;
                _cellData[freeCellData.Index] = freeCellData;
                SetupCellVisuals(freeCellData.Cell, freeCellData);
            }
            else
            {
                Debug.LogWarning("Нет свободных ячеек для нового предмета!");
            }
        }

        public void ClearCell(int index)
        {
            if (!_cellData.TryGetValue(index, out var cellData))
                return;

            cellData.Item = null;
            _cellData[index] = cellData;
            _freeCells.Enqueue(cellData);
            SetupCellVisuals(cellData.Cell, cellData);
        }

        public void Dispose()
        {
        }
    }
}
