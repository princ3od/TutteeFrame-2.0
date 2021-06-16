﻿using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutteeFrame2.DataAccess;
using TutteeFrame2.Reports.ReportModel;
using TutteeFrame2.View;

namespace TutteeFrame2.Reports.ReportControll
{
    class ReportStudentResultControll
    {
        public frmStudentResultReport frmStudentResultReport;
        private MaterialSurface.MaterialComboBox cbbSemester;
        private MaterialSurface.MaterialComboBox cbbFilterByClass;
        private List<String> cbbFilterByClassItems = new List<String>();
        private MaterialSurface.MaterialComboBox cbbFilterByGrade;
        private List<StudentResult> rawStudentResults;
        public List<StudentResult> studentResults;
        private List<StudentResult> studentResultsCache;
        private bool firstLoadSemester;
        private MaterialListView listViewStudentResult;
        public ReportStudentResultControll(frmStudentResultReport frmStudentResultReport,
            MaterialSurface.MaterialComboBox cbbFilterByClass,
            MaterialSurface.MaterialComboBox cbbFilterByGrade,
              MaterialSurface.MaterialComboBox cbbSemester,
              MaterialListView listViewStudentResult
            )
        {
            this.listViewStudentResult = listViewStudentResult;
            this.frmStudentResultReport = frmStudentResultReport;
            this.cbbFilterByGrade = cbbFilterByGrade;
            this.cbbFilterByClass = cbbFilterByClass;
            this.cbbSemester = cbbSemester;
            cbbFilterByGrade.SelectedIndex = 0;
            cbbFilterByClass.SelectedIndex = 0;
            cbbSemester.SelectedIndex = 0;

            // cbbGradeSelectedItem = frmStudentResultReport.
        }


        public void FetchData()
        {
            rawStudentResults = PrepareToPrintDA.instance.GetStudentResults();
            FilterByGrade();
        }
        private void FetchItemsCbbFilterByClass()
        {
            cbbFilterByClassItems.Clear();
            cbbFilterByClassItems.Add("Tất cả");
            foreach (var item in studentResults)
            {
                if (cbbFilterByClassItems.IndexOf(item.classID) == -1)
                {
                    cbbFilterByClassItems.Add(item.classID);
                }
            }
            cbbFilterByClass.Items.Clear();
            foreach (var item in cbbFilterByClassItems)
            {
                cbbFilterByClass.Items.Add(item);
            }
        }
        public void FilterByGrade()
        {
            firstLoadSemester = true;
            studentResults = new List<StudentResult>();
            if (cbbFilterByGrade.Text != "Tất cả")
            {
                foreach (var item in rawStudentResults)
                {
                    if (item.classID.Substring(0, 2) == cbbFilterByGrade.Text)
                    {
                        studentResults.Add(item);
                    }
                }
            }
            else
            {
                studentResults = rawStudentResults;
            }
            FetchItemsCbbFilterByClass();
            cbbFilterByClass.SelectedIndex = 0;
            cbbSemester.SelectedIndex = -1;
            frmStudentResultReport.ShowData();

        }
        public void FilterByClass()
        {
            firstLoadSemester = true;
            var results = new List<StudentResult>();
            if (cbbFilterByClass.Text != "Tất cả")
            {
                foreach (var item in rawStudentResults)
                {
                    if (item.classID.Substring(0, 4) == cbbFilterByClass.Text)
                    {
                        results.Add(item);
                    }
                }
                studentResults = results;
            }
            cbbSemester.SelectedIndex = -1;
            frmStudentResultReport.ShowData();
        }

        public void FilterBySemester()
        {
            if (cbbSemester.SelectedIndex == -1) return;
            var results = new List<StudentResult>();
            if (firstLoadSemester)
            {
                studentResultsCache = studentResults;
                firstLoadSemester = false;
            };
            studentResults = studentResultsCache;

            foreach (var item in studentResults)
            {
                if ("Kì " + item.semester.ToString() == cbbSemester.Text)
                {
                    results.Add(item);
                }
            }
            studentResults = results;

            frmStudentResultReport.ShowData();
        }

        public void ChangeTypeOfResult()
        {
            listViewStudentResult.Columns.Clear();
        }
    }
}
