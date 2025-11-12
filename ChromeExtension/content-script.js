class LinkedinJob {
    constructor(company, jobTitle, additionalInfo, description) {
        this.urlJob = window.location.href;
        this.company = company;
        this.jobTitle = jobTitle;
        this.jobLocal = additionalInfo[0];
        this.jobWhen = additionalInfo[1];
        this.jobCandidatesGross = additionalInfo[2];
        this.jobDescription = description;
    }

    logItem() {
        console.log(
            'company:' + this.company
            + '-----jobTitle:' + this.jobTitle
            + '-----jobLocal:' + this.jobLocal
            + '-----jobWhen:' + this.jobWhen
            //+ '-----jobCandidatesGross:' + this.jobCandidatesGross
        );
    }
}

class LinkedinJobList {
    constructor() {
        this.items = [];
    }

    addItem(item) {
        this.items.push(item);
    }

    logItens() {
        for (var i = 0; i < this.items.length; i++) {
            this.items[i].logItem();
        }
    }
}

function getCompanyName() {
    var jobInfoElement = document.getElementsByClassName('jobs-semantic-search-job-details-wrapper')[0];
    var jobInfoElementHeader = jobInfoElement.getElementsByClassName('job-details-jobs-unified-top-card__company-name')[0];
    var companyName = jobInfoElementHeader.getElementsByTagName('a')[0].innerText;
    return companyName;
}
function getJobTitle() {
    var jobInfoElement = document.getElementsByClassName('jobs-semantic-search-job-details-wrapper')[0];
    var jobInfoElementHeader = jobInfoElement.getElementsByClassName('t-24 job-details-jobs-unified-top-card__job-title')[0];
    var jobTitle = jobInfoElementHeader.getElementsByTagName('a')[0].innerText;
    return jobTitle;
}

function getJobAdditionalInfo() {
    var jobInfoElement = document.getElementsByClassName('job-details-jobs-unified-top-card__primary-description-container');
    var spans = jobInfoElement[0].getElementsByClassName('tvm__text tvm__text--low-emphasis');
    const returnArray = new Array(3);
    if (spans.length > 4) {
        returnArray[0] = spans[0].innerText;//Local
        returnArray[1] = spans[2].innerText;//Quando
        returnArray[2] = spans[4].innerText;//Candidaturas
    } else if (spans.length > 2) {
        returnArray[0] = spans[0].innerText;//Local
        returnArray[1] = spans[2].innerText;//Quando
        returnArray[2] = '';//Candidaturas
    } else if (spans.length > 0) {
        returnArray[0] = spans[0].innerText;//Local
        returnArray[1] = '';//Quando
        returnArray[2] = '';//Candidaturas
    }
   
    return returnArray;
}

function getJobDescription() {
    var jobInfoElement = document.getElementsByClassName('jobs-description__container');
    var description = jobInfoElement[0].getElementsByClassName('mt4');
    return description[0].innerText;
}

const sleep = (delay) => new Promise((resolve) => setTimeout(resolve, delay))

const pagination = async () => {
    var paginationElement = document.getElementsByClassName('jobs-search-pagination jobs-search-results-list__pagination p4');
    var buttons = paginationElement[0].getElementsByTagName('button');

    for (let i = 0; i < buttons.length; i++) {
        if (buttons[i].getAttribute('aria-label').split(" ").length==3) {
            if (buttons[i].getAttribute('aria-label').split(" ")[2] != 'anterior' &&
                buttons[i].getAttribute('aria-label').split(" ")[0] == 'Ver') {
                buttons[i].click();
                await sleep(5000);
                console.log('mudou de pagina');
                return true;
            }
        }        
    }
    return false;

}
var lastScrollTop = 0;
const handleScroll = async () => {
    var elementJobList = document.getElementsByClassName('KRCxWXDYdQyAujpEeppSstprdoQhjFMgvvrvYE');
    elementJobList[0].scrollTop = elementJobList[0].scrollHeight;
    console.log('desceu scroll');
    if (lastScrollTop != elementJobList[0].scrollTop) {
        lastScrollTop = elementJobList[0].scrollTop;
        await sleep(3000);
        await handleScroll();
    }
}

const handleScrapping = async () => {
    var returnPagination = true;
    while (returnPagination) {
        lastScrollTop = 0;
        await handleScroll();
        console.log('handleScroll');
        document.getElementsByClassName('KRCxWXDYdQyAujpEeppSstprdoQhjFMgvvrvYE')[0].scrollTop = 0;
        await runLinks();
        console.log('runLinks');
        returnPagination = await pagination();
        console.log('pagination:' + returnPagination);

        await sleep(3000);
    }
    alert('acabou'); 
}

const runLinks = async () => {
    var uls = document.querySelectorAll('ul');
    lis = uls[8].querySelectorAll('li');
    addJob();
    for (let i = 1; i < lis.length; i++) {
        divs = lis[i].querySelectorAll('div');
        if (divs.length == 19) {
            divs[1].click();

            await sleep(3000);
            addJob();
        }
    }

}

async function saveFile(title, json) {
    console.log('oi1');
    try {
        // Open the save file picker and get a FileSystemFileHandle
        console.log('oi2');
        const fileHandle = await window.showSaveFilePicker({
            suggestedName: title + '.json', // Suggested file name
            types: [{
                description: 'Json Files',
                accept: { 'text/plain': ['.json'] },
            }],
        });
        console.log('oi3');
        // Create a writable stream to write content to the file
        const writableStream = await fileHandle.createWritable();
        console.log('oi4');
        // Write the desired content (e.g., a string or Blob)
        await writableStream.write(json);
        console.log('oi5');
        // Close the stream to finalize the save operation
        await writableStream.close();
        console.log('oi6');
        console.log('File saved successfully!');
    } catch (error) {
        console.error('Error saving file:', error);
    }
}


function save() {
    var urlParams = new URLSearchParams(window.location.search);
    var myParameterValue = urlParams.get('keywords');
    var jsonContent = JSON.stringify(jobList);
    saveFile(myParameterValue.replace(' '), jsonContent);
}

function addJob() {
    var companyName = getCompanyName();
    var title = getJobTitle();
    var additionalInfo = getJobAdditionalInfo();
    var description = getJobDescription();
    var jobInfo = new LinkedinJob(
        companyName,
        title,
        additionalInfo,
        description
    );
    jobList.addItem(jobInfo);
    console.log('companyName: ' + companyName + ' - ' + 'title: ' + title);
}

var jobList = new LinkedinJobList();
handleScrapping();



