﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Griddly.Mvc
{
    public static class GriddlyFilterExtensions
    {
        public static GriddlyFilterBox FilterBox(this GriddlyColumn column, FilterDataType dataType = FilterDataType.Decimal, string field = null, string caption = null)
        {
            if (caption == null)
                caption = column.Caption;

            if (field == null)
                field = GetField(column);

            if (string.IsNullOrWhiteSpace(caption))
                throw new ArgumentNullException("caption", "Caption must be specified.");
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException("field", "Field must be specified.");

            return new GriddlyFilterBox()
            {
                Field = field,
                Caption = caption,
                DataType = dataType
            };
        }

        public static GriddlyFilterRange FilterRange(this GriddlyColumn column, FilterDataType dataType = FilterDataType.Decimal, string field = null, string fieldEnd = null, string caption = null)
        {
            if (caption == null)
                caption = column.Caption;

            if (field == null)
                field = GetField(column) + "Start";
            if (fieldEnd == null)
                fieldEnd = GetField(column) + "End";

            if (string.IsNullOrWhiteSpace(caption))
                throw new ArgumentNullException("caption", "Caption must be specified.");
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException("field", "Field must be specified.");
            if (string.IsNullOrWhiteSpace(fieldEnd))
                throw new ArgumentNullException("fieldEnd", "End field must be specified.");

            return new GriddlyFilterRange()
            {
                Field = field,
                FieldEnd = fieldEnd,
                Caption = caption,
                DataType = dataType
            };
        }

        public static GriddlyFilterList FilterList(this GriddlyColumn column, IEnumerable<SelectListItem> items, bool isMultiple = true, bool defaultSelectAll = true, string nullItemText = null, bool isNoneAll = true, string field = null, string caption = null)
        {
            if (caption == null)
                caption = column.Caption;

            if (field == null)
                field = GetField(column);

            if (string.IsNullOrWhiteSpace(caption))
                throw new ArgumentNullException("caption", "Caption must be specified.");
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException("field", "Field must be specified.");

            List<SelectListItem> itemsList = new List<SelectListItem>();

            if (!string.IsNullOrWhiteSpace(nullItemText))
                itemsList.Add(new SelectListItem() { Text = nullItemText, Value = "" });

            itemsList.AddRange(items);

            List<SelectListItem> selectableItemsList = new List<SelectListItem>();

            selectableItemsList.AddRange(itemsList.Where(x => !(x is SelectListItemGroup)));
            selectableItemsList.AddRange(itemsList.Where(x => x is SelectListItemGroup).SelectMany(x => ((SelectListItemGroup)x).Items));

            return new GriddlyFilterList()
            {
                Field = field,
                Items = itemsList,
                SelectableItems = selectableItemsList,
                Caption = caption,
                IsMultiple = isMultiple,
                IsNoneAll = isNoneAll,
                IsNullable = !string.IsNullOrWhiteSpace(nullItemText),
                DefaultSelectAll = defaultSelectAll
            };
        }

        static string GetField(GriddlyColumn column)
        {
            string value = null;

            if (column.SortField != null)
            {
                value = column.SortField.Split('.').Last();

                if (value.Length > 1)
                    value = char.ToLower(value[0]) + value.Substring(1);
            }

            return value;
        }

        public static GriddlyFilterList FilterEnum<T>(this GriddlyColumn column, bool isMultiple = true, bool defaultSelectAll = true, string nullItemText = null, bool isNoneAll = true, string field = null, string caption = null)
            where T : struct
        {
            return column.FilterList(Extensions.ToSelectListItems<T>().OrderBy(x => x.Text), isMultiple, defaultSelectAll, nullItemText, isNoneAll, field, caption);
        }

        public static GriddlyFilterList FilterEnum<T>(this GriddlyColumn column, IEnumerable<T> items, bool isMultiple = true, bool defaultSelectAll = true, string nullItemText = null, bool isNoneAll = true, string field = null, string caption = null)
            where T : struct
        {
            return column.FilterList(Extensions.ToSelectListItems(items).OrderBy(x => x.Text), isMultiple, defaultSelectAll, nullItemText, isNoneAll, field, caption);
        }

        public static GriddlyFilterList FilterBool(this GriddlyColumn column, string trueLabel = "Yes", string falseLabel = "No", string nullItemText = null, bool isMultiple = false, bool defaultSelectAll = false, bool isNoneAll = false, string field = null, string caption = null)
        {
            return column.FilterList(BuildBoolItems(trueLabel, falseLabel), isMultiple, defaultSelectAll, nullItemText, isNoneAll, field, caption);
        }

        static List<SelectListItem> BuildBoolItems(string trueLabel, string falseLabel)
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Value = "True", Text = trueLabel },
                new SelectListItem() { Value = "False", Text = falseLabel },
            };
        }
    }
}