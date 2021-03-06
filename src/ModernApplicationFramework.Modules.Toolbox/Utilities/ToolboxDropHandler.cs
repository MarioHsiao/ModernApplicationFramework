﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.Modules.Toolbox.Controls;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Utilities
{
    internal class ToolboxDropHandler : IDropTarget
    {
        public void DragOver(IDropInfo dropInfo)
        {

            dropInfo.DropTargetAdorner = null;
            var stringFlag = false;

            if (IsTextObjectInDragSource(dropInfo, out var dataObject))
            {
                dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
                dropInfo.DropTargetAdorner = typeof(ToolboxInsertAdorner);
                var format = dataObject.GetFormats().First();
                dropInfo.Data = ToolboxItem.CreateTextItem(new ToolboxItemData(format, dataObject.GetData(format)));
                stringFlag = true;
            }

            if (dropInfo.IsSameDragDropContextAsSource && !stringFlag)
            {
                //Restore original data
                dropInfo.Data = dropInfo.DragInfo.SourceItem;
            }


            DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);

            if (!CanDropToolboxItem(dropInfo, dropInfo.Data as IToolboxNode))
                dropInfo.Effects = DragDropEffects.None;

            if (dropInfo.IsSameDragDropContextAsSource)
            {
                if (dropInfo.DragInfo != null && dropInfo.TargetItem == dropInfo.DragInfo.SourceItem)
                    dropInfo.Effects = DragDropEffects.None;
                else
                {
                    if (!stringFlag && Equals(dropInfo.TargetCollection, dropInfo.DragInfo.SourceCollection))
                    {
                        var targetSource = dropInfo.TargetCollection.Cast<object>().ToList();
                        var indexTarget = targetSource.IndexOf(dropInfo.TargetItem);
                        var indexSource = targetSource.IndexOf(dropInfo.DragInfo.SourceItem);

                        if (indexTarget == -1)
                        {
                            if (indexSource == dropInfo.InsertIndex - 1 && (dropInfo.InsertPosition & RelativeInsertPosition.TargetItemCenter) == 0)
                                dropInfo.Effects = DragDropEffects.None;
                        }


                        if (indexSource == indexTarget + 1 && dropInfo.InsertPosition == RelativeInsertPosition.AfterTargetItem && (dropInfo.InsertPosition & RelativeInsertPosition.TargetItemCenter) == 0)
                            dropInfo.Effects = DragDropEffects.None;
                        if (indexSource == indexTarget - 1 && dropInfo.InsertPosition == RelativeInsertPosition.BeforeTargetItem)
                            dropInfo.Effects = DragDropEffects.None;
                    }
                }
            }

            if (dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter))
                dropInfo.DropTargetAdorner = null;
            else
            {
                dropInfo.DropTargetAdorner = dropInfo.Effects == DragDropEffects.None ? null : typeof(ToolboxInsertAdorner);
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            bool stringFlag = false;
            if (dropInfo.DragInfo == null && dropInfo.Data is IToolboxItem item && dropInfo.IsSameDragDropContextAsSource)
            {
                if (item.DataSource.Data.Format != DataFormats.Text)
                    return;
                stringFlag = true;
            }
            if (dropInfo.IsSameDragDropContextAsSource && !stringFlag)
            {
                //Restore original data
                dropInfo.Data = dropInfo.DragInfo.SourceItem;
            }
            if (stringFlag)
            {
                var destinationList = dropInfo.TargetCollection.TryGetList();
                if (destinationList != null)
                {
                    destinationList.Insert(dropInfo.InsertIndex, dropInfo.Data);

                    var selectDroppedItems = DragDrop.DragDrop.GetSelectDroppedItems(dropInfo.VisualTarget);
                    if (selectDroppedItems)
                    {
                        DefaultDropHandler.SelectDroppedItems(dropInfo, new List<object> { dropInfo.Data });
                    }
                }
            }
            else
            {
                DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }

        }

        public static bool CanDropToolboxItem(IDropInfo dropInfo, IToolboxNode item)
        {
            if (dropInfo.TargetItem == null && item != null && !(item is IToolboxCategory))
                return false;
            if (dropInfo.TargetItem is IToolboxCategory && item != null &&
                !(item is IToolboxCategory) && 
                !dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter))
                return false;
            if (dropInfo.TargetItem is IToolboxItem &&
                dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter))
                return false;
            return true;
        }

        public static bool IsTextObjectInDragSource(IDropInfo dropInfo, out IDataObject stringData)
        {
            stringData = null;
            if (dropInfo.DragInfo == null && dropInfo.Data is IDataObject dataObject &&
                dropInfo.IsSameDragDropContextAsSource)
            {
                if (!dataObject.GetDataPresent(DataFormats.Text))
                    return false;
                stringData = dataObject;
                return true;
            }
            return false;
        }
    }
}