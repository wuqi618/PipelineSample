node {
    stage('checkout') {
        // git 'https://github.com/wuqi618/PipelineSample.git'
        checkout scm
    }
    
    stage('build') {
        dir('src/WebApiSample/') {
            sh 'dotnet restore -r ubuntu.16.04-x64'
            sh 'dotnet build -c Release -r ubuntu.16.04-x64'
        }
    }
    
    stage('test') {
        dir('src/WebApiSample/Tests/') {
            sh 'dotnet xunit -c Release -xml TestResult/TestResult.xml'
        }
    }
    
    stage('publish') {
        dir('src/WebApiSample/') {
            sh 'dotnet publish WebApiSample/WebApiSample.csproj -c Release -r ubuntu.16.04-x64 -o Publish'
        }
    }
    
    stage('archive') {
        dir('src/WebApiSample/Tests/') {
            step([$class: 'XUnitPublisher', testTimeMargin: '3000', thresholdMode: 1, thresholds: [[$class: 'FailedThreshold', failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0'], [$class: 'SkippedThreshold', failureNewThreshold: '', failureThreshold: '', unstableNewThreshold: '', unstableThreshold: '']], tools: [[$class: 'XUnitDotNetTestType', deleteOutputFiles: true, failIfNotNew: true, pattern: 'TestResult/*.xml', skipNoTestFiles: false, stopProcessingIfError: true]]])
        }
        
        dir('src/WebApiSample/WebApiSample/Publish/') {
            archiveArtifacts '**'
        }
    }
}