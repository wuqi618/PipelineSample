node {
    stage('checkout') {
        // git 'https://github.com/wuqi618/PipelineSample.git'
        checkout scm
    }
    
    //stage('build') {
    //    dir('src/WebApiSample/') {
    //        sh 'dotnet restore -r ubuntu.16.04-x64'
    //        sh 'dotnet build -c Release -r ubuntu.16.04-x64'
    //    }
    //}
    
    // stage('test') {
    //     dir('src/WebApiSample/Tests/') {
    //         sh 'dotnet xunit -c Release -xml TestResult/TestResult.xml'
    //     }
    // }
    
    stage('publish') {
        dir('src/WebApiSample/WebApiSample/') {
            //sh 'rm -rf Publish'
            //sh 'dotnet publish WebApiSample.csproj -c Release -r ubuntu.16.04-x64 -o Publish'
            def output = sh returnStdout: true, script: 'aws ecr get-login --region ap-southeast-2'
            output = output.replaceFirst(" -e none ", "")
            sh "$output"
            sh '''output=$(aws ecr get-login --region ap-southeast-2)
                echo $output
                output=$(sed "s/ -e none / /g" <<<$output)
                eval $output
                docker build -t webapisample -f Dockerfile.ci .
                docker tag webapisample:latest 531585151505.dkr.ecr.ap-southeast-2.amazonaws.com/pipeline-sample-ecr:latest
                docker push 531585151505.dkr.ecr.ap-southeast-2.amazonaws.com/pipeline-sample-ecr:latest'''
        }
    }
    
    // stage('zip & s3') {
    //     dir('src/WebApiSample/WebApiSample/') {
    //         sh 'rm -f publish.zip'
    //         sh 'zip -r publish.zip Publish'
    //         sh "aws s3 cp publish.zip s3://webapisample-publish/publish-${BUILD_NUMBER}.zip"
    //     }
    // }
    
    stage('archive') {
        dir('src/WebApiSample/Tests/') {
            step([$class: 'XUnitPublisher', testTimeMargin: '3000', thresholdMode: 1, thresholds: [[$class: 'FailedThreshold', failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0'], [$class: 'SkippedThreshold', failureNewThreshold: '', failureThreshold: '', unstableNewThreshold: '', unstableThreshold: '']], tools: [[$class: 'XUnitDotNetTestType', deleteOutputFiles: true, failIfNotNew: true, pattern: 'TestResult/*.xml', skipNoTestFiles: false, stopProcessingIfError: true]]])
        }
        
        dir('src/WebApiSample/WebApiSample/Publish/') {
            archiveArtifacts '**'
        }
    }
}
