var options =
{
    series: [{ name: "Current", data: [18, 21, 45, 36, 65, 47, 51, 32, 40, 28, 31, 26] },
            { name: "Previous", data: [30, 11, 22, 18, 32, 23, 58, 45, 30, 36, 15, 34] }],
    chart:  { height: 350, type: "area", toolbar: { show: !1 } }, colors: ["#556ee6", "#f1b44c"],
    dataLabels: { enabled: !1 }, stroke: { curve: "smooth", width: 2 },
    fill: {
        type: "gradient",
        gradient: { shadeIntensity: 1, inverseColors: !1, opacityFrom: .45, opacityTo: .05, stops: [20, 100, 100, 100] }
    }, xaxis:
        { categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"] },
    markers: { size: 3, strokeWidth: 3, hover: { size: 4, sizeOffset: 2 } },
    legend: { position: "top", horizontalAlign: "right" }
}, chart = new
        ApexCharts(document.querySelector("#area-chart"),
            options); chart.render();





options =
{
    series:
        [{ name: "series1", data: [31, 40, 36, 51, 49, 72, 69, 56, 68, 82, 68, 76] }], chart: { height: 320, type: "line", toolbar: "false", dropShadow: { enabled: !0, color: "#000", top: 18, left: 7, blur: 8, opacity: .2 } }, dataLabels: { enabled: !1 }, colors: ["#556ee6"], stroke: { curve: "smooth", width: 3 }
}, chart = new ApexCharts(document.querySelector("#line-chart"), options); chart.render(); options = {
    series: [56, 38, 26], chart: { type: "donut", height: 240 }, labels: ["Block 5", "Block 6", "Block 7"], colors: ["#556ee6", "#34c38f", "#f46a6a"],
    legend: { show: !1 }, plotOptions: { pie: { donut: { size: "70%" } } }
}; (chart = new ApexCharts(document.querySelector("#donut-chart"),
    options)).render(); var radialoptions1 = {
        series: [37],
        chart:
        {
            type: "radialBar", width: 60, height: 60, sparkline:
                { enabled: !0 }
        }, dataLabels: { enabled: !1 }, colors: ["#556ee6"], plotOptions:
            { radialBar: { hollow: { margin: 0, size: "60%" }, track: { margin: 0 }, dataLabels: { show: !1 } } }
    }, radialchart1 =
        new ApexCharts(document.querySelector("#radialchart-1"), radialoptions1);


radialchart1.render(); var radialoptions2 = {
    series: [72], chart: {
        type: "radialBar", width: 60, height: 60, sparkline: { enabled: !0 }
    }, dataLabels: { enabled: !1 }, colors: ["#34c38f"], plotOptions:
     { radialBar: { hollow: { margin: 0, size: "60%" }, track: { margin: 0 }, dataLabels: { show: !1 } } }

}, radialchart2 = new ApexCharts(document.querySelector("#radialchart-2"),
    radialoptions2); radialchart2.render(); var radialoptions3 =
    {
        series: [54], chart: {
            type: "radialBar", width:
                60, height: 60, sparkline: { enabled: !0 }
        }, dataLabels: { enabled: !1 }, colors: ["#f46a6a"],
        plotOptions: { radialBar: { hollow: { margin: 0, size: "60%" }, track: { margin: 0 }, dataLabels: { show: !1 } } }
}, radialchart3 = new ApexCharts(document.querySelector("#radialchart-3"), radialoptions3); radialchart3.render(); 




options =
{
    chart: { height: 320, type: "pie" }, series: [52, 120, 44], labels:
        [" Opend ", "Closed", "Pending"], colors:
        ["#34c38f", "#556ee6", "#f46a6a"], legend: 
    {
        show: !0, position: "bottom", horizontalAlign: "center",
        verticalAlign: "middle", floating: !1, fontSize: "14px", offsetX: 0, offsetY: -10
    }, responsive: [{ breakpoint: 600, options: { chart: { height: 240 }, legend: { show: !1 } } }]
}; (chart = new ApexCharts(document.querySelector("#pie_chart"), options)).render();  



options =
{
    chart: { height: 320, type: "pie" }, series: [52, 120 ], labels:
        [" Approved ", "Not Approved" ], colors: 
        ["#34c38f", "#f46a6a" ], legend:
    {
        show: !0, position: "bottom", horizontalAlign: "center",
        verticalAlign: "middle", floating: !1, fontSize: "14px", offsetX: 0, offsetY: -10
    }, responsive: [{ breakpoint: 600, options: { chart: { height: 240 }, legend: { show: !1 } } }]
}; (chart = new ApexCharts(document.querySelector("#pie_chart1"), options)).render(); 



options =
{
    chart: { height: 320, type: "pie" }, series: [52, 120, 44], labels:
        ["Paid", "UnPaid", "Pending"], colors:
        ["#34c38f", "#556ee6", "#f46a6a"], legend:
    {
        show: !0, position: "bottom", horizontalAlign: "center",
        verticalAlign: "middle", floating: !1, fontSize: "14px", offsetX: 0, offsetY: -10
    }, responsive: [{ breakpoint: 600, options: { chart: { height: 240 }, legend: { show: !1 } } }]
}; (chart = new ApexCharts(document.querySelector("#pie_chart2"), options)).render();  