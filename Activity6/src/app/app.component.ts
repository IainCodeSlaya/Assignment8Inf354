import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Chart } from 'chart.js';
import { ReportingService } from './reporting.service';
import { mergeMap, groupBy, map, reduce} from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Activity6';

  chart=[];
  courses: Object=null;
  option = [
    {id: 1, text: "On Site Courses"},
    {id: 2, text: "Online Courses"},
    {id: 3, text: "All Courses"}
  ]
  selection : Number = 3;

 constructor (private reporting : ReportingService){}
 //constructor (private School : ReportingService){}

  random_rgba() {
    var o = Math.round, r = Math.random, s = 255;
    return 'rgba(' + o(r()*s) + ',' + o(r()*s) +',' + o(r()*s) +', 0.7)';
  }

  submitRequest() {
    this.reporting.getReportingData(this.selection).subscribe(response=> {
      console.log(response);

      let keys = response["Department"].map((d=>d.Name));
      let values = response["Department"].map((d=>d.Average));

      this.courses = response ['Courses'];

      this.chart = new Chart('canvas', {
        type: 'bar',
        data: {
          labels: keys, 
          datasets: [
            {
              data: values, 
              borderColor: "#3cba8f",
              fill: false,
              backgroundColor: [
                this.random_rgba(),
                this.random_rgba(),
                this.random_rgba(),
                this.random_rgba()
              ]
            }
          ]
        }
      })
    })
  }
}
