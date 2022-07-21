import $ from 'jquery';
import { saveAs } from 'file-saver';

// require the viewer, make sure you added it to your project
// dependencies via npm install --save-dev bpmn-js
import EmbeddedMenu from './menu';
import customControlsModule from './custom';


import CustomModeler from './custom-modeler';
import startData from '../resources/start-data.bpmn';
import minimapModule from 'diagram-js-minimap';
import AutoLayout from './auto-layout';
var modeler = new CustomModeler({
  container: '#canvas',
  additionalModules: [
    EmbeddedMenu,
    customControlsModule,
    minimapModule
  ],
  keyboard: {
    bindTo: document
  }
});


//----------------start------------------------------

(function () {
    var username=$("#UserName").val();
    var nameHistory= $("#HistoryName").val();
    var historyStart=$("#HistoryStart").val();
  var SD=localStorage.getItem(`${nameHistory}-${username}-${historyStart}`);
   if(SD){
    SD=JSON.parse(SD).xml;
    if(SD){
        importXML(SD,true);
    }else{
      openDiagram(startData);
    }
  }else{
    openDiagram(startData);
  }
})();
modeler.get('canvas').zoom('fit-viewport');


//-------------functions---------------


function importXML(diagram,repeatAutoLayout){
  modeler.importXML(diagram, function(err) {
    if (err) {
      if(repeatAutoLayout){
        var autoLayout = new AutoLayout();
        autoLayout.layoutProcess(diagram, function (err, xmlWithDi) {
          importXML(xmlWithDi);
        });
      }else{
        //alert('could not import BPMN 2.0 XML, see console');
        return console.log('could not import BPMN 2.0 XML', err);
      }
    }

    
    checkDiagram();
  });
}

// file save handling



function checkDiagram(){
  //console.log("checkDiagram");
  $(modeler._definitions.rootElements[0].flowElements).each((index,elem)=>{
    $(`[data-element-id=${elem.id}]`).removeClass("connect-error");
  });
  var empties=modeler._definitions.rootElements[0].flowElements;
  empties=empties.filter(e=>e.$type=='bpmn:UserTask'&&!e.$attrs["bpmn:sValue"]&&!e.$attrs["sValue"]);
var operations=modeler._definitions.rootElements[0].flowElements.filter(e=>e.$type=='bpmn:ServiceTask');
    var operationsOR=operations.filter(e=>e.$attrs["bpmn:sField"]=="OR"||e.$attrs["sField"]=="OR");
    operationsOR=operationsOR.filter(e=>!e.incoming||e.incoming.length==0||!e.outgoing||e.outgoing.length<2);
    var operationsAND=operations.filter(e=>e.$attrs["bpmn:sField"]=="AND"||e.$attrs["sField"]=="AND");
    operationsAND=operationsAND.filter(e=>!e.incoming||e.incoming.length==0||!e.outgoing||e.outgoing.length<2);
    var operationsNOT=operations.filter(e=>e.$attrs["bpmn:sField"]=="NOT"||e.$attrs["sField"]=="NOT");
    operationsNOT=operationsNOT.filter(e=>!e.incoming||e.incoming.length==0||!e.outgoing||e.outgoing.length!=1);

    var condition=modeler._definitions.rootElements[0].flowElements;
    condition=condition.filter(e=>e.$type=='bpmn:UserTask'&&(!e.incoming||e.incoming.length==0));

    var link=modeler._definitions.rootElements[0].flowElements;
    link=link.filter(e=>e.$type=='bpmn:ManualTask'&&(!e.incoming||e.incoming.length==0));

    var select=modeler._definitions.rootElements[0].flowElements;
    select=select.filter(e=>e.$type=='bpmn:Task'&&e.id=="Task_select"&&(!e.outgoing||e.outgoing.length<1));
   
    var errorMessage="";
    if(empties.length==0&&operationsOR.length==0&&operationsAND.length==0&&operationsNOT.length==0&&condition.length==0&&link.length==0){
      $(".fp_errorsSearch").html(errorMessage);
     
      return true;
    } else{
      if(condition.length!=0){
        $(condition).each((index,elem)=>{
          $(`[data-element-id=${elem.id}]`).addClass("connect-error");
        });
        errorMessage+="Condition has exactly one input and no output arrows. ";
      }
      if(link.length!=0){
        $(link).each((index,elem)=>{
          $(`[data-element-id=${elem.id}]`).addClass("connect-error");
        });
        errorMessage+="Link has exactly one input and one output arrows. ";
      }
      if(empties.length!=0){
        $(empties).each((index,elem)=>{
          $(`[data-element-id=${elem.id}]`).addClass("connect-error");
        });
        errorMessage+="Diagram contains empty blocks. ";
      }
      if(operationsOR.length!=0){
        $(operationsOR).each((index,elem)=>{
          $(`[data-element-id=${elem.id}]`).addClass("connect-error");
        });
        errorMessage+=`Operation "OR" must have one input arrow and two or more output arrows. `;
      }
      if(operationsAND.length!=0){
        $(operationsAND).each((index,elem)=>{
          $(`[data-element-id=${elem.id}]`).addClass("connect-error");
        });
        errorMessage+=`Operation "AND" must have one input arrow and two or more output arrows. `;
      }
      if(operationsNOT.length!=0){
        $(operationsNOT).each((index,elem)=>{
          $(`[data-element-id=${elem.id}]`).addClass("connect-error");
        });
        errorMessage+=`Operation "NOT" must have one input arrow and one output arrows. `;
      }
      // if(select.length!=0){
      //   $(select).each((index,elem)=>{
      //     $(`[data-element-id=${elem.id}]`).addClass("connect-error");
      //   });
      //   errorMessage+=`Search block must have one output arrow. `;
      // }
      errorMessage+="Please fill in the block(s)";
      $(".fp_errorsSearch").html(errorMessage);
    }
    return false;
}
function openDiagram(diagram) {
  //console.log("openDiagram");
  if(diagram&&diagram.search("bpmn:definitions")<0&&window["importToFile"]){
    importToFile(
      {
        xml:diagram,
        bpmnXml:startData,
        callbackSucceed:function(data, status, xhr, _args, response){
          var autoLayout = new AutoLayout();
          autoLayout.layoutProcess(response.result||_args.bpmnXml,function(err, xmlWithDi){
            importXML(xmlWithDi);
          });
          
        }
    });
  }else if(diagram){
    importXML(diagram);
  }
}
if(window["search"]){
  $(".fp_toolbar-buttons__button_search").on('click', function(){
    if(checkDiagram()==true){
      modeler.saveXML(function(err, xml) {
        var autoLayout = new AutoLayout();
        autoLayout.layoutProcess(xml,function(err, xmlWithDi){
          search({xml:xmlWithDi}); 
        });
      });
    }
  });
}
$(".fp_toolbar-buttons__button_new").on('click', function(){
  openDiagram(startData);
  if(window["newFile"]){
    newFile({});
  }
});
$(".fp_toolbar-buttons__button_export").on('click', function(){
  if(checkDiagram()==true&&window["exportToFile"]){
    modeler.saveXML(function(err, xml) {
      var autoLayout = new AutoLayout();
      autoLayout.layoutProcess(xml,function(err, xmlWithDi){
        exportToFile({xml:xmlWithDi,callbackSucceed: function (data, status, xhr, _args, response) {
          var blob = new Blob([response.result], { type: "application/xml;charset=utf-8" });
          saveAs(blob, "contact-advanced-search.xml");
      }}); 
      });
     
    });
  }else if(checkDiagram()==true){
    modeler.saveXML(function(err, xml) {
      var autoLayout = new AutoLayout();
      autoLayout.layoutProcess(xml,function(err, xmlWithDi){
        var blob = new Blob([xmlWithDi], {type: "application/bpmn20-xml;charset=utf-8"});
        saveAs(blob, "contact-advanced-search.bpmn");
      });
    });
  }
});
$(".fp_toolbar-buttons__button_undo").on("click", function (elem) {
  if(window["setHistoryStep"]&&!$(elem.target).hasClass("fp_toolbar-buttons__button_inactive")){
    setHistoryStep({ step: 1, callbackSucceed:function(xml){
        if(xml){
          importXML(xml,true);
        }else{
          openDiagram(startData);
        }
      } 
    });
  }
});
$(".fp_toolbar-buttons__button_redo").on("click", function (elem) {
  if(window["setHistoryStep"]&&!$(elem.target).hasClass("fp_toolbar-buttons__button_inactive")){
    setHistoryStep({ step: -1, callbackSucceed:function(xml){
        if(xml){
          importXML(xml,true);
        }else{
          openDiagram(startData);
        }
      } 
    });
  }
});
$(".fp_toolbar-buttons__button_import").on('click', function(){
  $('[data-open-file]').click();
});
$(".fp_toolbar-buttons__button_open").on('click', function(){
  if(window["openToBrowser"]){
    openToBrowser({
      callbackSucceed:function(data, status, xhr, _args, response){
        var autoLayout = new AutoLayout();
          autoLayout.layoutProcess(response.result||_args.bpmnXml,function(err, xmlWithDi){
            openDiagram(xmlWithDi);
          });
    }
  });
  }
});
$(".fp_toolbar-buttons__button_save").on('click', function(){
  if(checkDiagram()==true&&window["saveToFolder"]){
    modeler.saveXML(function(err, xml) {
        var autoLayout = new AutoLayout();
        autoLayout.layoutProcess(xml,function(err, xmlWithDi){
            saveToFolder({xml:xmlWithDi,callbackSucceed:function(data, status, xhr, _args, response){
            }});
        }); 
    });
  }
});

$(".fp_toolbar-buttons__button_layout").on('click', function(){
  modeler.saveXML(function(err, xml) {
    var autoLayout = new AutoLayout();
    autoLayout.layoutProcess(xml,function(err, xmlWithDi){
      importXML(xmlWithDi);
      modeler.get('canvas').zoom('fit-viewport');
    });
  });
});
function serialize() {
  modeler.saveXML(function(err, xml) {
    saveChanges(xml);
    if (err) {
      console.log('failed to serialize BPMN 2.0 xml', err);
    }
    openDiagram(xml);
  });
}
function update(){
  modeler.saveXML(function(err, xml) {
    saveChanges(xml);
    if (err) {
      console.log('failed to serialize BPMN 2.0 xml', err);
    }
  checkDiagram();
  });
}
function saveChanges(xml){
  if( window["saveXmlChanges"]){
      saveXmlChanges({xml:xml});
  }
}
modeler.on('menu.update', serialize);
modeler.on('fields.update', update);
modeler.on('commandStack.changed', serialize);




// file open handling

var $file = $('[data-open-file]');

function readFile(file, done) {

  if (!file) {
    return done(new Error('no file chosen'));
  }

  var reader = new FileReader();

  reader.onload = function(e) {
    done(null, e.target.result);
  };

  reader.readAsText(file);
}

$file.on('change', function() {
  readFile(this.files[0], function(err, xml) {

    if (err) {
      alert('could not read file, see console');
      return console.error('could not read file', err);
    }

    openDiagram(xml);
    $file.val("");
  });

});




function openFile(file, callback) {

  // check file api availability
  if (!window.FileReader) {
    return window.alert(
      'Looks like you use an older browser that does not support drag and drop. ' +
      'Try using a modern browser such as Chrome, Firefox or Internet Explorer > 10.');
  }

  // no file chosen
  if (!file) {
    return;
  }

  var reader = new FileReader();

  reader.onload = function(e) {

    var xml = e.target.result;

    callback(xml);
  };

  reader.readAsText(file);
}

(function onFileDrop(container, callback) {

  function handleFileSelect(e) {
    e.stopPropagation();
    e.preventDefault();

    var files = e.dataTransfer.files;
    openFile(files[0], callback);
  }

  function handleDragOver(e) {
    e.stopPropagation();
    e.preventDefault();

    e.dataTransfer.dropEffect = 'copy'; // Explicitly show this is a copy.
  }

  container.get(0).addEventListener('dragover', handleDragOver, false);
  container.get(0).addEventListener('drop', handleFileSelect, false);

})($('body'), openDiagram);


